namespace Crudspa.Content.Messaging.Client.Services;

public class SmsTemplateServiceTcp(IProxyWrappers proxyWrappers) : ISmsTemplateService
{
    public async Task<Response<IList<SmsTemplate>>> SearchForPortal(Request<SmsTemplateSearch> request) =>
        await proxyWrappers.Send<IList<SmsTemplate>>("SmsTemplateSearchForPortal", request);

    public async Task<Response<SmsTemplate?>> Fetch(Request<SmsTemplate> request) =>
        await proxyWrappers.Send<SmsTemplate?>("SmsTemplateFetch", request);

    public async Task<Response<SmsTemplate?>> Add(Request<SmsTemplate> request) =>
        await proxyWrappers.Send<SmsTemplate?>("SmsTemplateAdd", request);

    public async Task<Response> Save(Request<SmsTemplate> request) =>
        await proxyWrappers.Send("SmsTemplateSave", request);

    public async Task<Response> Remove(Request<SmsTemplate> request) =>
        await proxyWrappers.Send("SmsTemplateRemove", request);
}