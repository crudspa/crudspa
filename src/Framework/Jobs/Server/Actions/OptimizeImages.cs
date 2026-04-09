using ImageMagick;

namespace Crudspa.Framework.Jobs.Server.Actions;

public class OptimizeImages(
    ILogger<OptimizeImages> logger,
    IFrameworkActionService frameworkActionService,
    IBlobService blobService)
    : IJobAction
{
    private sealed record WebpProfile(
        String Name,
        Boolean Lossless,
        Int32 MasterQuality,
        Int32 ResizedQuality,
        Boolean UseUnsharp,
        Boolean Exact,
        Int32 AlphaQuality);

    private static readonly WebpProfile PhotoProfile = new("photo", false, 78, 82, true, false, 85);
    private static readonly WebpProfile GraphicProfile = new("graphic", true, 100, 100, false, true, 100);

    public OptimizeImagesConfig? Config { get; set; }
    private Guid? _sessionId;

    private const String RootFolder = @"C:\data\temp\images";

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<OptimizeImagesConfig>();
        _sessionId = sessionId;
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            await OptimizeImageFiles();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while optimizing images.");
            return false;
        }
    }

    private async Task OptimizeImageFiles()
    {
        logger.LogInformation("Fetching image files needing optimization...");

        var response = await frameworkActionService.FetchImageForOptimization(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to imageFileService.FetchForOptimization() failed. {response.ErrorMessages}");

        var imageFiles = response.Value;

        logger.LogInformation("Found {imageFilesCount} image files to optimize.", imageFiles.Count);

        if (imageFiles.Count == 0)
            return;

        var attempts = 0;
        var successes = 0;

        foreach (var imageFile in imageFiles)
        {
            attempts++;

            var downloadFileName = $"{imageFile.Id:D}{imageFile.Format}";
            var downloadFolder = Path.Combine(RootFolder, "download");
            var processedFolder = Path.Combine(RootFolder, "processed");

            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);

            if (!Directory.Exists(processedFolder))
                Directory.CreateDirectory(processedFolder);

            var downloadFilePath = Path.Combine(downloadFolder, downloadFileName);

            try
            {
                logger.LogInformation("File #{index}: Downloading and optimizing '{downloadPath}'...", attempts, downloadFilePath);

                await DownloadBlob(imageFile.BlobId!.Value, downloadFilePath);

                var (processedFiles, profile) = await OptimizeAndResize(downloadFilePath, processedFolder, imageFile);

                var sizeInfo = $"Profile: {profile.Name} | Original: {new FileInfo(downloadFilePath).Length:N0} B";

                if (processedFiles.TryGetValue("optimized", out var optimizedPath))
                    sizeInfo += $" | Optimized: {new FileInfo(optimizedPath).Length:N0} B";

                foreach (var key in processedFiles.Keys.Where(x => x != "optimized").OrderBy(Int32.Parse))
                    sizeInfo += $" | {key}: {new FileInfo(processedFiles[key]).Length:N0} B";

                logger.LogInformation("File {imageFileId} | {sizeInfo}", imageFile.Id, sizeInfo);

                foreach (var (sizeLabel, filePath) in processedFiles)
                {
                    var blobId = await AddBlob(filePath);

                    if (sizeLabel == "optimized")
                    {
                        imageFile.OptimizedBlobId = blobId;
                        imageFile.OptimizedFormat = filePath.GetExtension();
                    }
                    else
                    {
                        switch (sizeLabel)
                        {
                            case "96": imageFile.Resized96BlobId = blobId; break;
                            case "192": imageFile.Resized192BlobId = blobId; break;
                            case "360": imageFile.Resized360BlobId = blobId; break;
                            case "540": imageFile.Resized540BlobId = blobId; break;
                            case "720": imageFile.Resized720BlobId = blobId; break;
                            case "1080": imageFile.Resized1080BlobId = blobId; break;
                            case "1440": imageFile.Resized1440BlobId = blobId; break;
                            case "1920": imageFile.Resized1920BlobId = blobId; break;
                            case "3840": imageFile.Resized3840BlobId = blobId; break;
                        }
                    }

                    new FileInfo(filePath).SafeDelete();
                }

                imageFile.OptimizedStatus = ImageFile.OptimizationStatus.Succeeded;

                await frameworkActionService.SaveImageOptimizationStatus(new(_sessionId, imageFile));

                successes++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while optimizing image file '{imageFileId}'.", imageFile.Id);

                imageFile.OptimizedStatus = ImageFile.OptimizationStatus.Failed;

                await frameworkActionService.SaveImageOptimizationStatus(new(_sessionId, imageFile));
            }
            finally
            {
                new FileInfo(downloadFilePath).SafeDelete();
            }
        }

        logger.LogInformation("Command complete. {attempts} file(s) processed, {successes} successfully.", attempts, successes);
    }

    private static async Task<(Dictionary<String, String> Files, WebpProfile Profile)> OptimizeAndResize(String inputFilePath, String outputFolder, ImageFile imageFile)
    {
        var targetWidths = new[] { 96, 192, 360, 540, 720, 1080, 1440, 1920, 3840 };
        var results = new Dictionary<String, String>();

        var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
        var masterPath = Path.Combine(outputFolder, $"{fileName}-optimized.webp");

        using var original = await ToRaster(inputFilePath, imageFile, targetLargestWidth: 3840);

        imageFile.Width = (Int32?)original.Width;
        imageFile.Height = (Int32?)original.Height;

        Prepare(original);
        var profile = SelectProfile(imageFile, original);

        using (var master = new MagickImage(original))
        {
            ApplyProfile(master, profile, isMaster: true);
            await master.WriteAsync(masterPath);
            results["optimized"] = masterPath;
        }

        foreach (var width in targetWidths)
        {
            if (original.Width <= (UInt32)width) continue;

            using var resized = new MagickImage(original);

            resized.Resize((UInt32)width, 0);
            var resizedPath = Path.Combine(outputFolder, $"{fileName}-{width}.webp");

            ApplyProfile(resized, profile, isMaster: false);
            await resized.WriteAsync(resizedPath);
            results[width.ToString()] = resizedPath;
        }

        imageFile.OptimizedFormat = ".webp";

        return (results, profile);
    }

    private static async Task<MagickImage> ToRaster(String inputFilePath, ImageFile imageFile, Int32 targetLargestWidth)
    {
        if (!imageFile.Format.IsBasically(".svg"))
            return new(inputFilePath);

        var settings = new MagickReadSettings
        {
            BackgroundColor = MagickColors.Transparent,
            Width = (UInt32?)targetLargestWidth,
        };

        var image = new MagickImage();

        await image.ReadAsync(inputFilePath, settings);

        return image;
    }

    private static void Prepare(MagickImage image)
    {
        if (image.HasProfile("icc"))
            image.TransformColorSpace(ColorProfiles.SRGB);
        else
            image.ColorSpace = ColorSpace.sRGB;

        image.AutoOrient();
        image.Strip();
        image.Depth = 8;
    }

    private static WebpProfile SelectProfile(ImageFile imageFile, MagickImage image)
    {
        if (imageFile.Format.IsBasically(".svg"))
            return GraphicProfile;

        var totalColors = image.TotalColors;
        var colorDensity = totalColors / Math.Max(1D, image.Width * (Double)image.Height);

        return totalColors <= 4096 || (totalColors <= 32768 && colorDensity <= .01D)
            ? GraphicProfile
            : PhotoProfile;
    }

    private static void ApplyProfile(MagickImage image, WebpProfile profile, Boolean isMaster)
    {
        image.Format = MagickFormat.WebP;
        image.FilterType = FilterType.Lanczos;
        image.Quality = (UInt32)(isMaster ? profile.MasterQuality : profile.ResizedQuality);

        if (profile.UseUnsharp)
            image.UnsharpMask(0.0, 0.8, isMaster ? 0.3 : 0.4, 0.02);

        SetDefines(image, profile);
    }

    private static void SetDefines(MagickImage image, WebpProfile profile)
    {
        image.Settings.SetDefine("webp:method", "6");
        image.Settings.SetDefine("webp:auto-filter", "true");
        image.Settings.SetDefine("webp:lossless", profile.Lossless ? "true" : "false");
        image.Settings.SetDefine("webp:use-sharp-yuv", profile.Lossless ? "false" : "true");

        if (profile.Exact)
            image.Settings.SetDefine("webp:exact", "true");

        if (image.HasAlpha)
        {
            image.Settings.SetDefine("webp:alpha-compression", "1");
            image.Settings.SetDefine("webp:alpha-quality", $"{profile.AlphaQuality:D}");
        }
    }

    private async Task DownloadBlob(Guid blobId, String filePath)
    {
        var blob = await blobService.Fetch(new() { Id = blobId });

        if (blob?.Data is null)
            throw new($"No data found for blob id: {blobId}.");

        await File.WriteAllBytesAsync(filePath, blob.Data);
    }

    private async Task<Guid> AddBlob(String fileName)
    {
        var blobId = Guid.NewGuid();

        await using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
        await blobService.AddStream(blobId, fileStream);

        return blobId;
    }
}