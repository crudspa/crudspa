using System.Text;

namespace Crudspa.Framework.Core.Server.Services;

public class SmsSenderLocalFile : ISmsSender
{
    private readonly IServiceWrappers _wrappers;
    private readonly String _rootFolder;

    public SmsSenderLocalFile(IServiceWrappers wrappers)
    {
        _wrappers = wrappers;

        var home = Environment.GetEnvironmentVariable("HOME");

        _rootFolder = home.HasSomething()
            ? Path.Combine(home, "data", "temp", "sms")
            : @"c:\data\temp\sms";

        Directory.CreateDirectory(_rootFolder);
    }

    public async Task<Response> Send(Request<SmsOutboundMessage> request)
    {
        return await _wrappers.Try(request, async response =>
        {
            var message = request.Value;
            var messageId = message.Id ?? Guid.NewGuid();
            message.ProviderMessageId = messageId.ToString("D");
            var baseFileName = $"{DateTimeOffset.Now.AsFileName()}-{messageId:D}";
            var textFile = Path.Combine(_rootFolder, $"{baseFileName}.txt");

            await File.WriteAllTextAsync(textFile, ToText(message));

            var index = 0;
            foreach (var media in message.Media.Where(x => x.Data is not null))
            {
                index++;
                var extension = media.ContentType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/gif" => ".gif",
                    "image/webp" => ".webp",
                    _ => ".bin",
                };

                var mediaFile = Path.Combine(_rootFolder, $"{baseFileName}-media-{index}{extension}");
                await File.WriteAllBytesAsync(mediaFile, media.Data!);
            }
        });
    }

    private static String ToText(SmsOutboundMessage message)
    {
        var output = new StringBuilder();

        output.AppendLine($"Sms Channel: {message.SmsChannelKey}");
        output.AppendLine($"From: {message.From}");
        output.AppendLine($"To: {message.To}");
        output.AppendLine($"Provider Message Id: {message.ProviderMessageId}");
        output.AppendLine($"Body: {Environment.NewLine}{message.Body}");

        foreach (var media in message.Media)
        {
            output.AppendLine($"Media: {media.Name}");
            output.AppendLine($"Media Content Type: {media.ContentType}");
            output.AppendLine($"Media Url: {media.Url}");
        }

        if (message.StatusCallbackUrl.HasSomething())
            output.AppendLine($"Status Callback: {message.StatusCallbackUrl}");

        return output.ToString();
    }
}