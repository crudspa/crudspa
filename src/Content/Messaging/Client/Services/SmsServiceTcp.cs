namespace Crudspa.Content.Messaging.Client.Services;

public class SmsServiceTcp(IProxyWrappers proxyWrappers) : ISmsService
{
    public async Task<Response<IList<Sms>>> SearchForMembership(Request<SmsSearch> request) =>
        await proxyWrappers.Send<IList<Sms>>("SmsSearchForMembership", request);

    public async Task<Response<IList<Sms>>> SearchForPortal(Request<SmsSearch> request) =>
        await proxyWrappers.Send<IList<Sms>>("SmsSearchForPortal", request);

    public async Task<Response<Sms?>> Fetch(Request<Sms> request) =>
        await proxyWrappers.Send<Sms?>("SmsFetch", request);

    public async Task<Response<Sms?>> Add(Request<Sms> request) =>
        await proxyWrappers.Send<Sms?>("SmsAdd", request);

    public async Task<Response> Save(Request<Sms> request) =>
        await proxyWrappers.Send("SmsSave", request);

    public async Task<Response> Remove(Request<Sms> request) =>
        await proxyWrappers.Send("SmsRemove", request);

    public async Task<Response<IList<SmsTemplateFull>>> FetchSmsTemplates(Request<Portal> request) =>
        await proxyWrappers.Send<IList<SmsTemplateFull>>("SmsFetchSmsTemplates", request);
}