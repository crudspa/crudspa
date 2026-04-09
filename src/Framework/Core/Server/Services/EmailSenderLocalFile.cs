using System.Text;

namespace Crudspa.Framework.Core.Server.Services;

public class EmailSenderLocalFile : IEmailSender
{
    private readonly IServiceWrappers _wrappers;
    private readonly ICssInliner _cssInliner;
    private readonly String _rootFolder;

    public EmailSenderLocalFile(IServiceWrappers wrappers, ICssInliner cssInliner)
    {
        _wrappers = wrappers;
        _cssInliner = cssInliner;

        var home = Environment.GetEnvironmentVariable("HOME");

        _rootFolder = home.HasSomething()
            ? Path.Combine(home, "data", "temp", "email")
            : @"c:\data\temp\email";

        Directory.CreateDirectory(_rootFolder);
    }

    public async Task<Response> Send(Request<EmailMessage> request)
    {
        return await _wrappers.Try(request, async response =>
        {
            var email = request.Value;

            email.Message = _cssInliner.InlineCss(email.Message!);

            var messageId = email.MessageId ?? Guid.NewGuid();

            var file = Path.Combine(_rootFolder, $"{DateTimeOffset.Now.AsFileName()}-{messageId:D}.txt");
            var text = ToText(email);

            await File.WriteAllTextAsync(file, text);
        });
    }

    private static String ToText(EmailMessage emailMessage)
    {
        var output = new StringBuilder();

        output.AppendLine($"From: {emailMessage.From!.Name} ({emailMessage.From.Address})");

        if (emailMessage.ReplyTo is not null)
            output.AppendLine($"Reply To: {emailMessage.ReplyTo.Name} ({emailMessage.ReplyTo.Address})");

        foreach (var address in emailMessage.To)
            output.AppendLine($"To: {address.Name} ({address.Address})");

        foreach (var address in emailMessage.Cc)
            output.AppendLine($"CC: {address.Name} ({address.Address})");

        foreach (var address in emailMessage.Bcc)
            output.AppendLine($"BCC: {address.Name} ({address.Address})");

        foreach (var attachment in emailMessage.Attachments)
            output.AppendLine($"Attachment: {attachment.Name}");

        output.AppendLine($"Subject: {emailMessage.Subject}");

        output.AppendLine($"Message: {Environment.NewLine}{emailMessage.Message}");

        return output.ToString();
    }
}