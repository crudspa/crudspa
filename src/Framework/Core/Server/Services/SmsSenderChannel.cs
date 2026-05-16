namespace Crudspa.Framework.Core.Server.Services;

public class SmsSenderChannel(
    IServiceWrappers wrappers,
    ISmsChannelService smsChannelService,
    SmsSenderNull nullSender,
    SmsSenderLocalFile localFileSender,
    SmsSenderTwilio twilioSender)
    : ISmsSender
{
    public async Task<Response> Send(Request<SmsOutboundMessage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var message = request.Value;
            var channel = smsChannelService.Resolve(message.SmsChannelKey, message.PortalId);

            if (!channel.Enabled)
            {
                response.AddError($"SMS channel '{channel.Key}' is disabled.");
                return;
            }

            message.SmsChannelKey = channel.Key;
            message.From = message.From.HasSomething() ? message.From : channel.FromNumber;

            if (channel.RedirectOutgoingSms.HasSomething())
            {
                message.Body = $"[Redirected from {message.To}]{Environment.NewLine}{message.Body}";
                message.To = channel.RedirectOutgoingSms;
            }

            var provider = SmsChannelServiceCore.ResolveProvider(channel.Provider);

            var sendResponse = provider.ToLowerInvariant() switch
            {
                "twilio" => await twilioSender.Send(request),
                "localfile" => await localFileSender.Send(request),
                "null" => await nullSender.Send(request),
                _ => throw new($"SMS channel '{channel.Key}' uses an unknown provider '{channel.Provider}'."),
            };

            response.AddErrors(sendResponse.Errors);
        });
    }
}