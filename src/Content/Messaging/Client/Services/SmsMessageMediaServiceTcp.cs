namespace Crudspa.Content.Messaging.Client.Services;

public class SmsMessageMediaServiceTcp(IProxyWrappers proxyWrappers) : ISmsMessageMediaService
{
    public async Task<Response<IList<SmsMessageMedia>>> FetchForSmsMessage(Request<SmsMessage> request) =>
        await proxyWrappers.Send<IList<SmsMessageMedia>>("SmsMessageMediaFetchForSmsMessage", request);

    public async Task<Response> SaveOrder(Request<IList<SmsMessageMedia>> request) =>
        await proxyWrappers.Send("SmsMessageMediaSaveOrder", request);
}