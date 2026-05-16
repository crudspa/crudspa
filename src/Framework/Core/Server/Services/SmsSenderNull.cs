namespace Crudspa.Framework.Core.Server.Services;

public class SmsSenderNull(IServiceWrappers wrappers) : ISmsSender
{
    public async Task<Response> Send(Request<SmsOutboundMessage> request)
    {
        return await wrappers.Try(request, response =>
        {
            request.Value.ProviderMessageId = $"null-{Guid.NewGuid():D}";
            return Task.CompletedTask;
        });
    }
}