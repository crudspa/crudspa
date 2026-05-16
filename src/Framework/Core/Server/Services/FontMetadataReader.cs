using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace Crudspa.Framework.Core.Server.Services;

public class FontMetadata
{
    public String Style { get; set; } = "normal";
    public Int32 WeightMin { get; set; }
    public Int32 WeightMax { get; set; }
}

public static class FontMetadataReader
{
    private const String Os2Tag = "OS/2";
    private const String HeadTag = "head";
    private const String NameTag = "name";
    private const String FvarTag = "fvar";

    private static readonly String[] Woff2KnownTags =
    [
        "cmap", "head", "hhea", "hmtx", "maxp", "name", "OS/2", "post",
        "cvt ", "fpgm", "glyf", "loca", "prep", "CFF ", "VORG", "EBDT",
        "EBLC", "gasp", "hdmx", "kern", "LTSH", "PCLT", "VDMX", "vhea",
        "vmtx", "BASE", "GDEF", "GPOS", "GSUB", "EBSC", "JSTF", "MATH",
        "CBDT", "CBLC", "COLR", "CPAL", "SVG ", "sbix", "acnt", "avar",
        "bdat", "bloc", "bsln", "cvar", "fdsc", "feat", "fmtx", "fvar",
        "gvar", "hsty", "just", "lcar", "mort", "morx", "opbd", "prop",
        "trak", "Zapf", "Silf", "Glat", "Gloc", "Feat", "Sill",
    ];

    public static async Task<FontMetadata?> Read(Stream stream)
    {
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        return Read(memory.ToArray());
    }

    public static FontMetadata? Read(Byte[] data)
    {
        if (data.Length < 4)
            return null;

        var tables = Signature(data) switch
        {
            "wOFF" => ReadWoffTables(data),
            "wOF2" => ReadWoff2Tables(data),
            _ => ReadSfntTables(data),
        };

        if (tables is null || !tables.TryGetValue(Os2Tag, out var os2))
            return null;

        var os2Weight = ReadOs2Weight(os2);
        if (!os2Weight.HasValue)
            return null;

        var fvarWeight = tables.TryGetValue(FvarTag, out var fvar)
            ? ReadFvarWeight(fvar)
            : null;

        var style = ReadOs2Style(os2);
        if (style == "normal" && tables.TryGetValue(HeadTag, out var head))
            style = ReadHeadStyle(head);

        if (style == "normal" && tables.TryGetValue(FvarTag, out fvar))
            style = ReadFvarStyle(fvar);

        if (style == "normal" && tables.TryGetValue(NameTag, out var name))
            style = ReadNameStyle(name);

        return new()
        {
            Style = style,
            WeightMin = fvarWeight?.Min ?? os2Weight.Value,
            WeightMax = fvarWeight?.Max ?? os2Weight.Value,
        };
    }

    private static IReadOnlyDictionary<String, Byte[]>? ReadSfntTables(Byte[] data)
    {
        var fontOffset = 0;

        if (Signature(data) == "ttcf")
        {
            if (data.Length < 16)
                return null;

            fontOffset = ReadInt32(data, 12);
        }

        if (fontOffset < 0 || fontOffset + 12 > data.Length)
            return null;

        var numTables = ReadUInt16(data, fontOffset + 4);
        var directoryOffset = fontOffset + 12;
        var tables = new Dictionary<String, Byte[]>();

        for (var i = 0; i < numTables; i++)
        {
            var recordOffset = directoryOffset + i * 16;
            if (recordOffset + 16 > data.Length)
                return null;

            var tag = ReadTag(data, recordOffset);
            var tableOffset = ReadInt32(data, recordOffset + 8);
            var tableLength = ReadInt32(data, recordOffset + 12);

            if (!IsSafeRange(data, tableOffset, tableLength))
                return null;

            if (IsMetadataTag(tag))
                tables[tag] = data[tableOffset..(tableOffset + tableLength)];
        }

        return tables;
    }

    private static IReadOnlyDictionary<String, Byte[]>? ReadWoffTables(Byte[] data)
    {
        if (data.Length < 44)
            return null;

        var numTables = ReadUInt16(data, 12);
        var tables = new Dictionary<String, Byte[]>();

        for (var i = 0; i < numTables; i++)
        {
            var recordOffset = 44 + i * 20;
            if (recordOffset + 20 > data.Length)
                return null;

            var tag = ReadTag(data, recordOffset);
            var tableOffset = ReadInt32(data, recordOffset + 4);
            var compressedLength = ReadInt32(data, recordOffset + 8);
            var originalLength = ReadInt32(data, recordOffset + 12);

            if (!IsSafeRange(data, tableOffset, compressedLength))
                return null;

            if (!IsMetadataTag(tag))
                continue;

            var tableBytes = data[tableOffset..(tableOffset + compressedLength)];
            if (compressedLength != originalLength)
                tableBytes = InflateZlib(tableBytes, originalLength);

            if (tableBytes.Length != originalLength)
                return null;

            tables[tag] = tableBytes;
        }

        return tables;
    }

    private static IReadOnlyDictionary<String, Byte[]>? ReadWoff2Tables(Byte[] data)
    {
        if (data.Length < 48)
            return null;

        var flavor = Signature(data, 4);
        var numTables = ReadUInt16(data, 12);
        var totalCompressedSize = ReadInt32(data, 20);
        var offset = 48;
        var entries = new List<Woff2TableEntry>();

        for (var i = 0; i < numTables; i++)
        {
            if (offset >= data.Length)
                return null;

            var flags = data[offset++];
            var tagIndex = flags & 0x3f;
            var tag = tagIndex == 0x3f
                ? ReadExplicitWoff2Tag(data, ref offset)
                : Woff2KnownTags[tagIndex];
            var transformVersion = flags >> 6;
            var originalLength = ReadUIntBase128(data, ref offset);

            if (originalLength < 0 || HasUnknownWoff2Transform(tag, transformVersion))
                return null;

            var transformed = HasWoff2Transform(tag, transformVersion);
            var streamLength = transformed ? ReadUIntBase128(data, ref offset) : originalLength;
            if (streamLength < 0)
                return null;

            entries.Add(new(tag, streamLength));
        }

        var includedTableIndexes = ReadWoff2CollectionIndexes(data, flavor, ref offset);
        if (flavor == "ttcf" && includedTableIndexes is null)
            return null;

        if (!IsSafeRange(data, offset, totalCompressedSize))
            return null;

        var decompressed = DecompressBrotli(data[offset..(offset + totalCompressedSize)]);
        var tables = new Dictionary<String, Byte[]>();
        var tableOffset = 0;

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (!IsSafeRange(decompressed, tableOffset, entry.StreamLength))
                return null;

            if ((includedTableIndexes is null || includedTableIndexes.Contains(i)) && IsMetadataTag(entry.Tag))
                tables[entry.Tag] = decompressed[tableOffset..(tableOffset + entry.StreamLength)];

            tableOffset += entry.StreamLength;
        }

        return tables;
    }

    private static HashSet<Int32>? ReadWoff2CollectionIndexes(Byte[] data, String flavor, ref Int32 offset)
    {
        if (flavor != "ttcf")
            return null;

        if (offset + 4 > data.Length)
            return null;

        offset += 4;
        var numFonts = Read255UInt16(data, ref offset);
        if (numFonts <= 0)
            return null;

        HashSet<Int32>? firstFontIndexes = null;

        for (var fontIndex = 0; fontIndex < numFonts; fontIndex++)
        {
            var numTables = Read255UInt16(data, ref offset);
            if (numTables < 0 || offset + 4 > data.Length)
                return null;

            offset += 4;
            var indexes = new HashSet<Int32>();

            for (var tableIndex = 0; tableIndex < numTables; tableIndex++)
            {
                var index = Read255UInt16(data, ref offset);
                if (index < 0)
                    return null;

                indexes.Add(index);
            }

            firstFontIndexes ??= indexes;
        }

        return firstFontIndexes;
    }

    private static Boolean HasUnknownWoff2Transform(String tag, Int32 transformVersion)
    {
        return tag switch
        {
            "glyf" or "loca" => transformVersion is not 0 and not 3,
            "hmtx" => transformVersion is not 0 and not 1,
            _ => transformVersion != 0,
        };
    }

    private static Boolean HasWoff2Transform(String tag, Int32 transformVersion)
    {
        return tag switch
        {
            "glyf" or "loca" => transformVersion != 3,
            "hmtx" => transformVersion == 1,
            _ => false,
        };
    }

    private static String ReadExplicitWoff2Tag(Byte[] data, ref Int32 offset)
    {
        if (offset + 4 > data.Length)
            return String.Empty;

        var tag = ReadTag(data, offset);
        offset += 4;
        return tag;
    }

    private static Int32? ReadOs2Weight(Byte[] os2)
    {
        if (os2.Length < 8)
            return null;

        var weight = ReadUInt16(os2, 4);
        return weight is >= 1 and <= 1000 ? weight : null;
    }

    private static String ReadOs2Style(Byte[] os2)
    {
        if (os2.Length < 64)
            return "normal";

        var fsSelection = ReadUInt16(os2, 62);
        var isItalic = (fsSelection & 0x0001) != 0;
        var isOblique = (fsSelection & 0x0200) != 0;
        return isItalic || isOblique ? "italic" : "normal";
    }

    private static String ReadHeadStyle(Byte[] head)
    {
        if (head.Length < 46)
            return "normal";

        var macStyle = ReadUInt16(head, 44);
        return (macStyle & 0x0002) != 0 ? "italic" : "normal";
    }

    private static WeightRange? ReadFvarWeight(Byte[] fvar)
    {
        var axis = ReadFvarAxes(fvar).FirstOrDefault(x => x.Tag == "wght");
        if (axis.Tag is null)
            return null;

        var min = ClampWeight(axis.Min);
        var max = ClampWeight(axis.Max);
        return min <= max ? new(min, max) : new(max, min);
    }

    private static String ReadFvarStyle(Byte[] fvar)
    {
        foreach (var axis in ReadFvarAxes(fvar))
        {
            if (axis.Tag == "ital" && axis.Default >= 0.5)
                return "italic";

            if (axis.Tag == "slnt" && Math.Abs(axis.Default) > 0.01)
                return "italic";
        }

        return "normal";
    }

    private static IEnumerable<VariationAxis> ReadFvarAxes(Byte[] fvar)
    {
        if (fvar.Length < 16)
            yield break;

        var axesOffset = ReadUInt16(fvar, 4);
        var axisCount = ReadUInt16(fvar, 8);
        var axisSize = ReadUInt16(fvar, 10);

        if (axisSize < 20)
            yield break;

        for (var i = 0; i < axisCount; i++)
        {
            var offset = axesOffset + i * axisSize;
            if (offset + 20 > fvar.Length)
                yield break;

            yield return new(
                ReadTag(fvar, offset),
                ReadFixed16Dot16(fvar, offset + 4),
                ReadFixed16Dot16(fvar, offset + 8),
                ReadFixed16Dot16(fvar, offset + 12));
        }
    }

    private static String ReadNameStyle(Byte[] name)
    {
        foreach (var value in ReadNameValues(name, 17).Concat(ReadNameValues(name, 2)).Concat(ReadNameValues(name, 4)))
        {
            if (value.Contains("Italic", StringComparison.OrdinalIgnoreCase) ||
                value.Contains("Oblique", StringComparison.OrdinalIgnoreCase))
                return "italic";
        }

        return "normal";
    }

    private static IEnumerable<String> ReadNameValues(Byte[] name, UInt16 nameId)
    {
        if (name.Length < 6)
            yield break;

        var count = ReadUInt16(name, 2);
        var stringOffset = ReadUInt16(name, 4);

        for (var i = 0; i < count; i++)
        {
            var recordOffset = 6 + i * 12;
            if (recordOffset + 12 > name.Length)
                yield break;

            if (ReadUInt16(name, recordOffset + 6) != nameId)
                continue;

            var platformId = ReadUInt16(name, recordOffset);
            var length = ReadUInt16(name, recordOffset + 8);
            var offset = ReadUInt16(name, recordOffset + 10);
            var valueOffset = stringOffset + offset;

            if (!IsSafeRange(name, valueOffset, length))
                continue;

            var valueBytes = name[valueOffset..(valueOffset + length)];
            var value = platformId is 0 or 3
                ? Encoding.BigEndianUnicode.GetString(valueBytes)
                : Encoding.Latin1.GetString(valueBytes);

            if (value.HasSomething())
                yield return value;
        }
    }

    private static Byte[] InflateZlib(Byte[] data, Int32 originalLength)
    {
        using var input = new MemoryStream(data);
        using var zlib = new ZLibStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream(originalLength);
        zlib.CopyTo(output);
        return output.ToArray();
    }

    private static Byte[] DecompressBrotli(Byte[] data)
    {
        using var input = new MemoryStream(data);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return output.ToArray();
    }

    private static Int32 ReadUIntBase128(Byte[] data, ref Int32 offset)
    {
        UInt32 value = 0;

        for (var i = 0; i < 5; i++)
        {
            if (offset >= data.Length)
                return -1;

            var b = data[offset++];
            if (i == 0 && b == 0x80)
                return -1;

            if ((value & 0xfe000000) != 0)
                return -1;

            value = (value << 7) | (UInt32)(b & 0x7f);
            if ((b & 0x80) == 0)
                return value <= Int32.MaxValue ? (Int32)value : -1;
        }

        return -1;
    }

    private static Int32 Read255UInt16(Byte[] data, ref Int32 offset)
    {
        if (offset >= data.Length)
            return -1;

        var code = data[offset++];
        return code switch
        {
            253 when offset + 2 <= data.Length => (data[offset++] << 8) | data[offset++],
            254 when offset < data.Length => data[offset++] + 506,
            255 when offset < data.Length => data[offset++] + 253,
            < 253 => code,
            _ => -1,
        };
    }

    private static Int32 ClampWeight(Double weight)
    {
        return Math.Clamp((Int32)Math.Round(weight), 1, 1000);
    }

    private static Double ReadFixed16Dot16(Byte[] data, Int32 offset)
    {
        return ReadInt32(data, offset) / 65536D;
    }

    private static UInt16 ReadUInt16(Byte[] data, Int32 offset)
    {
        return BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));
    }

    private static Int32 ReadInt32(Byte[] data, Int32 offset)
    {
        return BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(offset, 4));
    }

    private static String Signature(Byte[] data, Int32 offset = 0)
    {
        return ReadTag(data, offset);
    }

    private static String ReadTag(Byte[] data, Int32 offset)
    {
        return Encoding.ASCII.GetString(data, offset, 4);
    }

    private static Boolean IsSafeRange(Byte[] data, Int32 offset, Int32 length)
    {
        return offset >= 0 && length >= 0 && offset <= data.Length && length <= data.Length - offset;
    }

    private static Boolean IsMetadataTag(String tag)
    {
        return tag is Os2Tag or HeadTag or NameTag or FvarTag;
    }

    private readonly record struct WeightRange(Int32 Min, Int32 Max);
    private readonly record struct VariationAxis(String Tag, Double Min, Double Default, Double Max);
    private readonly record struct Woff2TableEntry(String Tag, Int32 StreamLength);
}