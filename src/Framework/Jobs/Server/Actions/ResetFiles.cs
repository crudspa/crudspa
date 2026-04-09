namespace Crudspa.Framework.Jobs.Server.Actions;

public class ResetFiles(
    ILogger<ResetFiles> logger,
    IFrameworkActionService frameworkActionService)
    : IJobAction
{
    public ResetFilesConfig? Config { get; set; }
    private Guid? _sessionId;

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<ResetFilesConfig>();
        _sessionId = sessionId;
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            if (Config is null)
                return false;

            if (Config.OptimizedAudioFiles == true)
                await ResetAudioOptimization();

            if (Config.OptimizedImageFiles == true)
                await ResetImageOptimization();

            if (Config.ImageCaptions == true)
                await ResetImageCaptions();

            if (Config.OptimizedVideoFiles == true)
                await ResetVideoOptimization();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while resetting files.");
            return false;
        }
    }

    private async Task ResetAudioOptimization()
    {
        logger.LogInformation("Beginning to reset audio optimization...");

        var response = await frameworkActionService.FetchAudioBeenOptimized(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to audioFileService.FetchBeenOptimized() failed. {response.ErrorMessages}");

        var audioFiles = response.Value;

        logger.LogInformation("Found {audioFilesCount} audio files to reset.", audioFiles.Count);

        var attempts = 0;
        var successes = 0;

        foreach (var audioFile in audioFiles)
        {
            attempts++;

            audioFile.OptimizedStatus = AudioFile.OptimizationStatus.None;
            audioFile.OptimizedBlobId = null;
            audioFile.OptimizedFormat = null;

            var updateResponse = await frameworkActionService.SaveAudioOptimizationStatus(new(_sessionId, audioFile));

            if (updateResponse.Ok)
                successes++;

            if (attempts % 500 == 0)
                logger.LogInformation("{attempts} audio files processed so far. {successes} successful updates.", attempts, successes);
        }

        logger.LogInformation("Done resetting audio optimization. {successes} files reset out of {attempts} attempts.", successes, attempts);
    }

    private async Task ResetImageOptimization()
    {
        logger.LogInformation("Beginning to reset image optimization...");

        var response = await frameworkActionService.FetchImageBeenOptimized(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to imageFileService.FetchBeenOptimized() failed. {response.ErrorMessages}");

        var imageFiles = response.Value;

        logger.LogInformation("Found {imageFilesCount} image files to reset.", imageFiles.Count);

        var attempts = 0;
        var successes = 0;

        foreach (var imageFile in imageFiles)
        {
            attempts++;

            imageFile.OptimizedStatus = ImageFile.OptimizationStatus.None;
            imageFile.OptimizedFormat = null;

            var updateResponse = await frameworkActionService.SaveImageOptimizationStatus(new(_sessionId, imageFile));

            if (updateResponse.Ok)
                successes++;

            if (attempts % 500 == 0)
                logger.LogInformation("{attempts} image files processed so far. {successes} successful updates.", attempts, successes);
        }

        logger.LogInformation("Done resetting image optimization. {successes} files reset out of {attempts} attempts.", successes, attempts);
    }

    private async Task ResetImageCaptions()
    {
        logger.LogInformation("Beginning to reset image captions...");

        var response = await frameworkActionService.FetchImageBeenOptimized(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to imageFileService.FetchBeenOptimized() failed. {response.ErrorMessages}");

        var imageFiles = response.Value.Where(x => !String.IsNullOrWhiteSpace(x.Caption)).ToList();

        logger.LogInformation("Found {imageFilesCount} image captions to reset.", imageFiles.Count);

        var attempts = 0;
        var successes = 0;

        foreach (var imageFile in imageFiles)
        {
            attempts++;

            imageFile.Caption = null;

            var updateResponse = await frameworkActionService.SaveImageCaption(new(_sessionId, imageFile));

            if (updateResponse.Ok)
                successes++;

            if (attempts % 500 == 0)
                logger.LogInformation("{attempts} image captions processed so far. {successes} successful updates.", attempts, successes);
        }

        logger.LogInformation("Done resetting image captions. {successes} captions reset out of {attempts} attempts.", successes, attempts);
    }

    private async Task ResetVideoOptimization()
    {
        logger.LogInformation("Beginning to reset video optimization...");

        var response = await frameworkActionService.FetchVideoBeenOptimized(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to videoFileService.FetchBeenOptimized() failed. {response.ErrorMessages}");

        var videoFiles = response.Value;

        logger.LogInformation("Found {videoFilesCount} video files to reset.", videoFiles.Count);

        var attempts = 0;
        var successes = 0;

        foreach (var videoFile in videoFiles)
        {
            attempts++;

            videoFile.OptimizedStatus = VideoFile.OptimizationStatus.None;
            videoFile.OptimizedFormat = null;

            var updateResponse = await frameworkActionService.SaveVideoOptimizationStatus(new(_sessionId, videoFile));

            if (updateResponse.Ok)
                successes++;

            if (attempts % 500 == 0)
                logger.LogInformation("{attempts} video files processed so far. {successes} successful updates.", attempts, successes);
        }

        logger.LogInformation("Done resetting video optimization. {successes} files reset out of {attempts} attempts.", successes, attempts);
    }
}