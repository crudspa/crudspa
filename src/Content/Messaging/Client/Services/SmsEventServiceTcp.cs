namespace Crudspa.Content.Messaging.Client.Services;

public class SmsEventServiceTcp(IProxyWrappers proxyWrappers) : ISmsEventService
{
    public async Task<Response<IList<SmsEvent>>> Search(Request<SmsEventSearch> request) =>
        await proxyWrappers.Send<IList<SmsEvent>>("SmsEventSearch", request);

    public async Task<Response<IList<SmsEvent>>> SearchForSmsMessage(Request<SmsEventSearch> request) =>
        await proxyWrappers.Send<IList<SmsEvent>>("SmsEventSearchForSmsMessage", request);
}