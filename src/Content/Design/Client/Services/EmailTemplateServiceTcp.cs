namespace Crudspa.Content.Design.Client.Services;

public class EmailTemplateServiceTcp(IProxyWrappers proxyWrappers) : IEmailTemplateService
{
    public async Task<Response<IList<EmailTemplate>>> SearchForMembership(Request<EmailTemplateSearch> request) =>
        await proxyWrappers.Send<IList<EmailTemplate>>("EmailTemplateSearchForMembership", request);

    public async Task<Response<EmailTemplate?>> Fetch(Request<EmailTemplate> request) =>
        await proxyWrappers.Send<EmailTemplate?>("EmailTemplateFetch", request);

    public async Task<Response<EmailTemplate?>> Add(Request<EmailTemplate> request) =>
        await proxyWrappers.Send<EmailTemplate?>("EmailTemplateAdd", request);

    public async Task<Response> Save(Request<EmailTemplate> request) =>
        await proxyWrappers.Send("EmailTemplateSave", request);

    public async Task<Response> Remove(Request<EmailTemplate> request) =>
        await proxyWrappers.Send("EmailTemplateRemove", request);

    public async Task<Response<IList<Token>>> FetchTokens(Request<Membership> request) =>
        await proxyWrappers.Send<IList<Token>>("EmailTemplateFetchTokens", request);
}