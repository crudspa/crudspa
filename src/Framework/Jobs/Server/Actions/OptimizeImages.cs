using Crudspa.Framework.Jobs.Server.Services;

namespace Crudspa.Framework.Jobs.Server.Actions;

public class OptimizeImages(
    ILogger<OptimizeImages> logger,
    IFrameworkActionService frameworkActionService,
    IBlobService blobService)
    : IJobAction
{
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

                var result = await ImageOptimizer.OptimizeAndResize(downloadFilePath, processedFolder, imageFile.Format);

                imageFile.Width = result.Width;
                imageFile.Height = result.Height;
                imageFile.OptimizedFormat = result.Format;

                var sizeInfo = $"Profile: {result.ProfileName} | Original: {new FileInfo(downloadFilePath).Length:N0} B";

                if (result.Files.TryGetValue("optimized", out var optimizedPath))
                    sizeInfo += $" | Optimized: {new FileInfo(optimizedPath).Length:N0} B";

                foreach (var key in result.Files.Keys.Where(x => x != "optimized").OrderBy(Int32.Parse))
                    sizeInfo += $" | {key}: {new FileInfo(result.Files[key]).Length:N0} B";

                logger.LogInformation("File {imageFileId} | {sizeInfo}", imageFile.Id, sizeInfo);

                foreach (var (sizeLabel, filePath) in result.Files)
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