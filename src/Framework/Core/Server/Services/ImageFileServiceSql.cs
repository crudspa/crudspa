using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ImageMagick;

namespace Crudspa.Framework.Core.Server.Services;

public class ImageFileServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService)
    : IImageFileService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<ImageFile?>> Fetch(Request<ImageFile> request)
    {
        return await wrappers.Try<ImageFile?>(request, async response =>
            await ImageFileSelect.Execute(Connection, request.Value));
    }

    public async Task<Response<ImageFile?>> Add(Request<ImageFile> request)
    {
        return await wrappers.Validate<ImageFile?, ImageFile>(request, async response =>
        {
            var imageFile = request.Value;
            await PopulateDimensions(imageFile);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                imageFile.Id = await ImageFileInsert.Execute(connection, transaction, request.SessionId, imageFile));

            return await ImageFileSelect.Execute(Connection, imageFile);
        });
    }

    public async Task<Response> Save(Request<ImageFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var imageFile = request.Value;

            var existing = await ImageFileSelect.Execute(Connection, imageFile);

            if (existing is not null)
            {
                if (imageFile.BlobId != existing.BlobId)
                {
                    await blobService.Remove(new Blob { Id = existing.BlobId });
                    imageFile.Caption = null;
                }
                else
                    imageFile.Caption = existing.Caption;
            }
            else
                imageFile.Caption = null;

            await PopulateDimensions(imageFile, existing);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await ImageFileUpdate.Execute(connection, transaction, request.SessionId, imageFile));
        });
    }

    public async Task<Response> Remove(Request<ImageFile> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var imageFile = request.Value;

            var existingImageFile = await ImageFileSelect.Execute(Connection, imageFile);

            if (existingImageFile is not null)
            {
                await blobService.Remove(new Blob { Id = existingImageFile.BlobId });

                await sqlWrappers.WithConnection(async (connection, transaction) =>
                    await ImageFileDelete.Execute(connection, transaction, request.SessionId, imageFile));
            }
        });
    }

    private async Task PopulateDimensions(ImageFile imageFile, ImageFile? existing = null)
    {
        if (imageFile.Width is > 0 && imageFile.Height is > 0)
            return;

        if (existing is not null
            && existing.BlobId == imageFile.BlobId
            && existing.Width is > 0 and { } existingWidth
            && existing.Height is > 0 and { } existingHeight)
        {
            imageFile.Width = existingWidth;
            imageFile.Height = existingHeight;
            return;
        }

        if (!imageFile.BlobId.HasValue)
            return;

        var blob = await blobService.Fetch(new() { Id = imageFile.BlobId });

        if (blob?.Data is not { Length: > 0 } data)
            return;

        var dimensions = imageFile.Format.IsBasically(".svg")
            ? TryReadSvgDimensions(data)
            : TryReadRasterDimensions(data);

        if (dimensions.HasValue)
        {
            imageFile.Width = dimensions.Value.Width;
            imageFile.Height = dimensions.Value.Height;
        }
    }

    private static (Int32 Width, Int32 Height)? TryReadRasterDimensions(Byte[] data)
    {
        try
        {
            var info = new MagickImageInfo(data);
            return info.Width > 0 && info.Height > 0
                ? ((Int32)info.Width, (Int32)info.Height)
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static (Int32 Width, Int32 Height)? TryReadSvgDimensions(Byte[] data)
    {
        try
        {
            using var stream = new MemoryStream(data, writable: false);
            var document = XDocument.Load(stream);
            var root = document.Root;

            if (root is null)
                return null;

            var viewBox = root.Attribute("viewBox")?.Value;

            if (viewBox.HasSomething())
            {
                var values = viewBox!
                    .Split([',', ' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Double.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : (Double?)null)
                    .ToList();

                if (values.Count == 4 && values[2] is > 0 and { } width && values[3] is > 0 and { } height)
                    return ((Int32)Math.Round(width), (Int32)Math.Round(height));
            }

            var widthValue = ParseSvgLength(root.Attribute("width")?.Value);
            var heightValue = ParseSvgLength(root.Attribute("height")?.Value);

            return widthValue is > 0 && heightValue is > 0
                ? ((Int32)Math.Round(widthValue.Value), (Int32)Math.Round(heightValue.Value))
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static Double? ParseSvgLength(String? value)
    {
        if (value.HasNothing())
            return null;

        var match = Regex.Match(value!, @"[-+]?\d*\.?\d+");

        if (!match.Success)
            return null;

        return Double.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }
}