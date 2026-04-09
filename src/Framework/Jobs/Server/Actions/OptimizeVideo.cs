using System.Text.Json;
using CliWrap;
using CliWrap.Buffered;
using Crudspa.Framework.Jobs.Server.Extensions;

namespace Crudspa.Framework.Jobs.Server.Actions;

public class OptimizeVideo(
    ILogger<OptimizeVideo> logger,
    IFrameworkActionService frameworkActionService,
    IBlobService blobService)
    : IJobAction
{
    public OptimizeVideoConfig? Config { get; set; }
    private Guid? _sessionId;

    private const String RootFolder = @"C:\data\temp\video";

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<OptimizeVideoConfig>();
        _sessionId = sessionId;
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            await OptimizeVideoFiles();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while optimizing video.");
            return false;
        }
    }

    private async Task OptimizeVideoFiles()
    {
        logger.LogInformation("Fetching video files needing optimization, dimension backfill, or poster generation...");

        var response = await frameworkActionService.FetchVideoForOptimization(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to videoFileService.FetchForOptimization() failed. {response.ErrorMessages}");

        var videoFiles = response.Value;

        logger.LogInformation("Found {videoFilesCount} video files to process.", videoFiles.Count);

        if (videoFiles.Count == 0)
            return;

        logger.LogInformation("Ensuring ffmpeg exists in '{folder}'...", RootFolder);

        await FfmpegHelpers.InstallTools(RootFolder);

        var attempts = 0;
        var successes = 0;
        var skippedPreviouslyFailed = 0;

        foreach (var videoFile in videoFiles)
        {
            if (ShouldSkipPreviouslyFailed(videoFile))
            {
                skippedPreviouslyFailed++;
                continue;
            }

            attempts++;

            var requiresOptimization = RequiresOptimization(videoFile);
            var needsDimensions = NeedsDimensions(videoFile);
            var requiresPoster = requiresOptimization || NeedsPoster(videoFile);
            var downloadFileName = $"{videoFile.Id:D}{FetchDownloadFormat(videoFile, requiresOptimization)}";
            var processedFileName = $"{videoFile.Id:D}.mp4";
            var posterFileName = $"{videoFile.Id:D}.jpg";

            var downloadFolder = Path.Combine(RootFolder, "download");
            var processedFolder = Path.Combine(RootFolder, "processed");

            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);
            if (!Directory.Exists(processedFolder))
                Directory.CreateDirectory(processedFolder);

            var downloadFilePath = Path.Combine(downloadFolder, downloadFileName);
            var processedFilePath = Path.Combine(processedFolder, processedFileName);
            var posterFilePath = Path.Combine(processedFolder, posterFileName);

            try
            {
                if (requiresOptimization)
                {
                    logger.LogInformation("File #{index}: Downloading and optimizing video file '{downloadPath}'...", attempts, downloadFilePath);

                    await DownloadAndOptimize(videoFile.BlobId!.Value, downloadFilePath, processedFilePath, videoFile);

                    var blobId = await AddBlob(processedFilePath);

                    videoFile.OptimizedBlobId = blobId;
                    videoFile.OptimizedFormat = ".mp4";
                    videoFile.OptimizedStatus = VideoFile.OptimizationStatus.Succeeded;
                }
                else
                {
                    var blobId = FetchBlobIdForDimensionBackfill(videoFile);

                    if (needsDimensions)
                    {
                        logger.LogInformation("File #{index}: Backfilling video dimensions for '{videoFileId}' from blob '{blobId}'.", attempts, videoFile.Id, blobId);
                        await BackfillDimensions(blobId, downloadFilePath, videoFile);
                    }
                    else
                    {
                        logger.LogInformation("File #{index}: Downloading existing optimized video '{videoFileId}' from blob '{blobId}' for poster generation.", attempts, videoFile.Id, blobId);
                        await DownloadBlob(blobId, downloadFilePath);
                    }
                }

                if (requiresPoster)
                {
                    try
                    {
                        var posterSourceFilePath = requiresOptimization
                            ? processedFilePath
                            : downloadFilePath;

                        logger.LogInformation("File #{index}: Generating poster image for '{videoFileId}'.", attempts, videoFile.Id);

                        await GeneratePoster(posterSourceFilePath, posterFilePath);

                        videoFile.PosterBlobId = await AddBlob(posterFilePath);
                        videoFile.PosterFormat = ".jpg";
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Poster generation failed for video file '{videoFileId}'. Continuing without poster.", videoFile.Id);
                    }
                }

                await frameworkActionService.SaveVideoOptimizationStatus(new(_sessionId, videoFile));

                successes++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while optimizing video file '{videoFileId}'.", videoFile.Id);

                if (requiresOptimization)
                {
                    videoFile.OptimizedStatus = VideoFile.OptimizationStatus.Failed;

                    await frameworkActionService.SaveVideoOptimizationStatus(new(_sessionId, videoFile));
                }
            }
            finally
            {
                new FileInfo(downloadFilePath).SafeDelete();
                new FileInfo(processedFilePath).SafeDelete();
                new FileInfo(posterFilePath).SafeDelete();
            }
        }

        logger.LogInformation("Command complete. {attempts} file(s) processed, {successes} successfully, {skippedPreviouslyFailed} skipped because they already failed optimization.", attempts, successes, skippedPreviouslyFailed);
    }

    private static Boolean RequiresOptimization(VideoFile videoFile)
    {
        return videoFile.OptimizedStatus == VideoFile.OptimizationStatus.None
            || !videoFile.OptimizedBlobId.HasValue
            || videoFile.OptimizedFormat.HasNothing();
    }

    private static Boolean ShouldSkipPreviouslyFailed(VideoFile videoFile)
    {
        return videoFile.OptimizedStatus == VideoFile.OptimizationStatus.Failed
            && !videoFile.OptimizedBlobId.HasValue;
    }

    private static Boolean NeedsPoster(VideoFile videoFile)
    {
        return !videoFile.PosterBlobId.HasValue
            || videoFile.PosterFormat.HasNothing();
    }

    private static Boolean NeedsDimensions(VideoFile videoFile)
    {
        return !videoFile.Width.HasValue
            || videoFile.Width <= 0
            || !videoFile.Height.HasValue
            || videoFile.Height <= 0;
    }

    private static String FetchDownloadFormat(VideoFile videoFile, Boolean requiresOptimization)
    {
        if (requiresOptimization)
            return videoFile.Format ?? ".mp4";

        return videoFile.OptimizedFormat.HasSomething()
            ? videoFile.OptimizedFormat!
            : videoFile.Format ?? ".mp4";
    }

    private static Guid FetchBlobIdForDimensionBackfill(VideoFile videoFile)
    {
        if (videoFile.OptimizedBlobId.HasValue)
            return videoFile.OptimizedBlobId.Value;

        if (videoFile.BlobId.HasValue)
            return videoFile.BlobId.Value;

        throw new($"No blob id available to backfill dimensions for video file '{videoFile.Id}'.");
    }

    private async Task DownloadAndOptimize(Guid blobId, String downloadFilePath, String processedFilePath, VideoFile videoFile)
    {
        await DownloadBlob(blobId, downloadFilePath);

        var (width, height) = await ProbeDimensions(downloadFilePath);

        videoFile.Width = width;
        videoFile.Height = height;

        var exePath = Path.Combine(RootFolder, "ffmpeg.exe");
        var arguments = $"-loglevel error -y -i \"{downloadFilePath}\" -movflags +faststart -c:v libx264 -preset medium -crf 23 -profile:v high -level 4.1 -pix_fmt yuv420p -c:a aac -ac 2 -b:a 192k \"{processedFilePath}\"";

        var command = Cli.Wrap(exePath)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None);

        var success = await command.RunAndLog(logger);

        if (!success)
            throw new($"FFmpeg failed for downloaded file '{downloadFilePath}' to processed file '{processedFilePath}'.");
    }

    private static async Task<(Int32 Width, Int32 Height)> ProbeDimensions(String filePath)
    {
        var exePath = Path.Combine(RootFolder, "ffprobe.exe");

        var args = $"-v error -select_streams v:0 -show_entries stream=width,height,side_data_list,rotation:stream_tags=rotate -of json \"{filePath}\"";

        var result = await Cli.Wrap(exePath)
            .WithArguments(args)
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0 || result.StandardOutput.HasNothing())
            throw new($"ffprobe failed to read stream info for '{filePath}'.");

        using var doc = JsonDocument.Parse(result.StandardOutput);
        var root = doc.RootElement;

        var streams = root.TryGetProperty("streams", out var sEl) && sEl.ValueKind == JsonValueKind.Array ? sEl : default;
        if (streams.ValueKind != JsonValueKind.Array || streams.GetArrayLength() == 0)
            throw new($"ffprobe returned no streams for '{filePath}'.");

        var stream = streams[0];

        var width = stream.TryGetProperty("width", out var wEl) && wEl.TryGetInt32(out var wVal) ? wVal : 0;
        var height = stream.TryGetProperty("height", out var hEl) && hEl.TryGetInt32(out var hVal) ? hVal : 0;

        var rotation = 0;

        if (stream.TryGetProperty("tags", out var tags)
            && tags.TryGetProperty("rotate", out var rotTag)
            && rotTag.ValueKind == JsonValueKind.String
            && Int32.TryParse(rotTag.GetString(), out var rotFromTag))
            rotation = rotFromTag;

        if (rotation == 0 && stream.TryGetProperty("rotation", out var rotEl) && rotEl.TryGetInt32(out var rotFromField))
            rotation = rotFromField;

        if (rotation == 0 && stream.TryGetProperty("side_data_list", out var sdl) && sdl.ValueKind == JsonValueKind.Array)
            foreach (var sd in sdl.EnumerateArray())
                if (sd.TryGetProperty("rotation", out var rotSd) && rotSd.TryGetInt32(out var rotFromSd))
                {
                    rotation = rotFromSd;
                    break;
                }

        rotation = (rotation % 360 + 360) % 360;

        if (rotation is 90 or 270)
            return (height, width);

        return (width, height);
    }

    private async Task BackfillDimensions(Guid blobId, String downloadFilePath, VideoFile videoFile)
    {
        await DownloadBlob(blobId, downloadFilePath);

        var (width, height) = await ProbeDimensions(downloadFilePath);

        videoFile.Width = width;
        videoFile.Height = height;
    }

    private async Task GeneratePoster(String inputFilePath, String posterFilePath)
    {
        var exePath = Path.Combine(RootFolder, "ffmpeg.exe");
        var arguments = $"-loglevel error -y -i \"{inputFilePath}\" -frames:v 1 -q:v 2 \"{posterFilePath}\"";

        var command = Cli.Wrap(exePath)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None);

        var success = await command.RunAndLog(logger);

        if (!success)
            throw new($"FFmpeg failed to extract poster from '{inputFilePath}' to '{posterFilePath}'.");
    }

    private async Task DownloadBlob(Guid videoFileId, String filePath)
    {
        var blob = await blobService.Fetch(new() { Id = videoFileId });

        if (blob?.Data is null)
            throw new($"No data found for blob id: {videoFileId}.");

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