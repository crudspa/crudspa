using System.Text.Json.Serialization;

namespace Crudspa.Framework.Core.Shared.Extensions;

public static class SerializationEx
{
    extension<T>(T input) where T : class
    {
        public T DeepClone()
        {
            var json = input.ToJson();
            return json?.FromJson<T>()!;
        }
    }

    extension(Object input)
    {
        public T? DownCast<T>() where T : class
        {
            var json = input.ToJson();
            return json?.FromJson<T>();
        }

        public String? ToJson(Boolean pretty = false)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = pretty,
            };

            return JsonSerializer.Serialize(input, options);
        }
    }

    extension(String? input)
    {
        public T? FromJson<T>() where T : class
        {
            if (input.HasNothing())
                return null;

            return JsonSerializer.Deserialize<T>(input);
        }

        public Object? FromJson(String typeName)
        {
            if (input.HasNothing() || typeName.HasNothing())
                return null;

            return JsonSerializer.Deserialize(input, Type.GetType(typeName)!);
        }

        public Object? FromJson()
        {
            if (input.HasNothing())
                return null;

            return JsonSerializer.Deserialize<Object>(input);
        }

        public Byte[] FromHexString()
        {
            var length = input!.Length;
            var bytes = new Byte[length / 2];

            for (var i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);

            return bytes;
        }
    }

    extension(Byte[] bytes)
    {
        public String ToHexString()
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
    }
}