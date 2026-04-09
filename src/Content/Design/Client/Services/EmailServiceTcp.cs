using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Client.Services;

public class EmailServiceTcp(IProxyWrappers proxyWrappers) : IEmailService
{
    public async Task<Response<IList<Email>>> SearchForMembership(Request<EmailSearch> request) =>
        await proxyWrappers.Send<IList<Email>>("EmailSearchForMembership", request);

    public async Task<Response<Email?>> Fetch(Request<Email> request) =>
        await proxyWrappers.Send<Email?>("EmailFetch", request);

    public async Task<Response<Email?>> Add(Request<Email> request) =>
        await proxyWrappers.Send<Email?>("EmailAdd", request);

    public async Task<Response> Save(Request<Email> request) =>
        await proxyWrappers.Send("EmailSave", request);

    public async Task<Response> Remove(Request<Email> request) =>
        await proxyWrappers.Send("EmailRemove", request);

    public async Task<Response<IList<EmailTemplateFull>>> FetchEmailTemplates(Request<Membership> request) =>
        await proxyWrappers.Send<IList<EmailTemplateFull>>("EmailFetchEmailTemplates", request);

    public async Task<Response<IList<Token>>> FetchTokens(Request<Membership> request) =>
        await proxyWrappers.Send<IList<Token>>("EmailFetchTokens", request);
}