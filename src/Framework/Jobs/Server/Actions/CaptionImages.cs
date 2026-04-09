using System.Text;
using System.Text.Json;

namespace Crudspa.Framework.Jobs.Server.Actions;

public class CaptionImages(
    ILogger<CaptionImages> logger,
    IFrameworkActionService frameworkActionService,
    IBlobService blobService,
    IJobsConfigService configService)
    : IJobAction
{
    public CaptionImagesConfig? Config { get; set; }
    private Guid? _sessionId;

    private const String RootFolder = @"C:\data\temp\images";
    private const String Model = "gpt-4.1-mini";
    private const String ChatCompletionsUrl = "https://api.openai.com/v1/chat/completions";

    private const String CaptionPrompt =
        "Write one sentence of alt text for a grade-school web app. " +
        "Describe only important visible details like people, objects, actions, and setting. " +
        "If text appears in the image, include that exact text. " +
        "Do not mention missing details. " +
        "Do not start with 'Image of', 'Photo of', 'This image shows', or 'Image description'.";

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<CaptionImagesConfig>();
        _sessionId = sessionId;
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            await CaptionImageFiles();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while captioning images.");
            return false;
        }
    }

    private async Task CaptionImageFiles()
    {
        logger.LogInformation("Fetching image files needing captioning...");

        var config = configService.Fetch();

        if (config.OpenAiApiKey.HasNothing())
            throw new("OpenAI API key is missing.");

        var response = await frameworkActionService.FetchImageForCaptioning(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to IImageFileService.FetchForCaptioning() failed. {response.ErrorMessages}");

        var imageFiles = response.Value;

        logger.LogInformation("Found {imageFilesCount} image files to caption.", imageFiles.Count);

        if (imageFiles.Count == 0)
            return;

        var attempts = 0;
        var successes = 0;

        foreach (var imageFile in imageFiles)
        {
            attempts++;

            var downloadFileName = $"{imageFile.Id:D}{imageFile.OptimizedFormat}";
            var downloadFolder = Path.Combine(RootFolder, "download");

            if (!Directory.Exists(downloadFolder))
                Directory.CreateDirectory(downloadFolder);

            var downloadFilePath = Path.Combine(downloadFolder, downloadFileName);

            try
            {
                logger.LogInformation("File #{index}: Downloading and captioning '{downloadPath}'...", attempts, downloadFilePath);

                await DownloadBlob(imageFile.OptimizedBlobId!.Value, downloadFilePath);

                var imageCaption = await GetCaption(downloadFilePath, imageFile.OptimizedFormat, config.OpenAiApiKey);

                if (!String.IsNullOrWhiteSpace(imageCaption))
                {
                    imageFile.Caption = imageCaption.Trim();
                    await frameworkActionService.SaveImageCaption(new(_sessionId, imageFile));
                    successes++;
                }
                else
                {
                    logger.LogWarning("Caption request did not return text for image file '{imageFileId}'.", imageFile.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while captioning image file '{imageFileId}'.", imageFile.Id);
            }
            finally
            {
                new FileInfo(downloadFilePath).SafeDelete();
            }
        }

        logger.LogInformation("Command complete. {attempts} file(s) processed, {successes} successfully.", attempts, successes);
    }

    private async Task<String?> GetCaption(String inputFilePath, String? imageFormat, String openAiApiKey)
    {
        const Int32 maxRetries = 20;
        var retryCount = 0;
        var base64Image = Convert.ToBase64String(await File.ReadAllBytesAsync(inputFilePath));
        var mimeType = GetMimeType(imageFormat);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");

        var requestBody = new
        {
            model = Model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new Object[]
                    {
                        new { type = "text", text = CaptionPrompt },
                        new { type = "image_url", image_url = new { url = $"data:{mimeType};base64,{base64Image}", detail = "low" } },
                    },
                },
            },
            max_tokens = 100,
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);

        while (retryCount < maxRetries)
        {
            try
            {
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(ChatCompletionsUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    retryCount++;
                    var retryAfter = 5000;

                    if (response.Headers.TryGetValues("Retry-After", out var values))
                        if (Int32.TryParse(values.FirstOrDefault(), out var parsedRetry))
                            retryAfter = parsedRetry * 1000;

                    logger.LogWarning("Rate limit hit. Retrying {retryCount}/{maxRetries} in {retryAfter} ms.", retryCount, maxRetries, retryAfter);
                    await Task.Delay(retryAfter);
                    continue;
                }

                response.EnsureSuccessStatusCode();

                using var doc = JsonDocument.Parse(responseBody);
                var description = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return description;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while captioning image file '{inputFilePath}', attempt {retryCount} of {maxRetries}.", inputFilePath, retryCount, maxRetries);
                retryCount++;
                await Task.Delay(5000);
            }
        }

        return null;
    }

    private static String GetMimeType(String? imageFormat)
    {
        return imageFormat?.ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "image/jpeg",
        };
    }

    private async Task DownloadBlob(Guid blobId, String filePath)
    {
        var blob = await blobService.Fetch(new() { Id = blobId });

        if (blob?.Data is null)
            throw new($"No data found for blob id: {blobId}.");

        await File.WriteAllBytesAsync(filePath, blob.Data);
    }
}