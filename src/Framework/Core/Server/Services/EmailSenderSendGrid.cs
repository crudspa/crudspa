using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;
using Response = Crudspa.Framework.Core.Shared.Contracts.Data.Response;

namespace Crudspa.Framework.Core.Server.Services;

public class EmailSenderSendGrid : IEmailSender
{
    private static readonly TimeSpan SendInterval = TimeSpan.FromMilliseconds(100);
    private readonly SemaphoreSlim _sendGate = new(1, 1);
    private DateTimeOffset _nextSend = DateTimeOffset.MinValue;

    private readonly IServiceWrappers _wrappers;
    private readonly ICssInliner _cssInliner;
    private readonly SendGridClient _apiClient;

    public EmailSenderSendGrid(IServiceWrappers wrappers,
        IServerConfigService configService,
        ICssInliner cssInliner)
    {
        _wrappers = wrappers;
        _cssInliner = cssInliner;

        var config = configService.Fetch();
        _apiClient = new(config.SendGridApiKey);
    }

    public async Task<Response> Send(Request<EmailMessage> request)
    {
        return await _wrappers.Try(request, async response =>
        {
            var email = request.Value;

            email.Message = _cssInliner.InlineCss(email.Message!);
            var mail = BuildSendGridMessage(email);

            await Throttle();

            var apiResponse = await _apiClient.SendEmailAsync(mail);

            if (apiResponse.StatusCode != HttpStatusCode.Accepted)
                response.AddError($"Call to SendGrid API failed. StatusCode: {apiResponse.StatusCode}. Response: {await apiResponse.Body.ReadAsStringAsync()}");
        });
    }

    private async Task Throttle()
    {
        await _sendGate.WaitAsync();

        try
        {
            var now = DateTimeOffset.UtcNow;

            if (now < _nextSend)
                await Task.Delay(_nextSend - now);

            _nextSend = DateTimeOffset.UtcNow.Add(SendInterval);
        }
        finally
        {
            _sendGate.Release();
        }
    }

    private static SendGridMessage BuildSendGridMessage(EmailMessage emailMessage)
    {
        var message = new SendGridMessage();

        message.SetClickTracking(false, false);
        message.SetOpenTracking(false);
        message.SetSubscriptionTracking(false);

        message.SetFrom(new(emailMessage.From!.Address, emailMessage.From.Name));
        message.SetSubject(emailMessage.Subject);
        message.AddContent("text/html", emailMessage.Message);

        foreach (var emailAddress in emailMessage.To)
            message.AddTo(new EmailAddress(emailAddress.Address, emailAddress.Name));

        foreach (var emailAddress in emailMessage.Cc)
            message.AddCc(new EmailAddress(emailAddress.Address, emailAddress.Name));

        foreach (var emailAddress in emailMessage.Bcc)
            message.AddBcc(new EmailAddress(emailAddress.Address, emailAddress.Name));

        if (emailMessage.ReplyTo is not null)
            message.SetReplyTo(new(emailMessage.ReplyTo.Address, emailMessage.ReplyTo.Name));

        foreach (var attachment in emailMessage.Attachments)
            if (attachment.Data is not null)
                message.AddAttachment(attachment.Name, Convert.ToBase64String(attachment.Data));

        return message;
    }
}