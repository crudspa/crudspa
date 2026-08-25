namespace Crudspa.Content.Messaging.Client.Services;

public class SmsMessageServiceTcp(IProxyWrappers proxyWrappers) : ISmsMessageService
{
    public async Task<Response<IList<SmsMessage>>> SearchForPortal(Request<SmsMessageSearch> request) =>
        await proxyWrappers.Send<IList<SmsMessage>>("SmsMessageSearchForPortal", request);

    public async Task<Response<IList<SmsMessage>>> SearchForMembership(Request<SmsMessageSearch> request) =>
        await proxyWrappers.Send<IList<SmsMessage>>("SmsMessageSearchForMembership", request);

    public async Task<Response<IList<SmsMessage>>> SearchForContactPhone(Request<SmsMessageSearch> request) =>
        await proxyWrappers.Send<IList<SmsMessage>>("SmsMessageSearchForContactPhone", request);

    public async Task<Response<IList<SmsMessage>>> SearchForContact(Request<SmsMessageSearch> request) =>
        await proxyWrappers.Send<IList<SmsMessage>>("SmsMessageSearchForContact", request);

    public async Task<Response<IList<SmsMessage>>> SearchThread(Request<SmsMessage> request) =>
        await proxyWrappers.Send<IList<SmsMessage>>("SmsMessageSearchThread", request);

    public async Task<Response<SmsMessage?>> Fetch(Request<SmsMessage> request) =>
        await proxyWrappers.Send<SmsMessage?>("SmsMessageFetch", request);

    public async Task<Response<SmsMessage?>> Reply(Request<SmsMessage> request) =>
        await proxyWrappers.Send<SmsMessage?>("SmsMessageReply", request);
}