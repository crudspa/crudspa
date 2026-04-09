namespace Crudspa.Content.Design.Client.Services;

public class StyleServiceTcp(IProxyWrappers proxyWrappers) : IStyleService
{
    public async Task<Response<IList<Style>>> FetchForContentPortal(Request<ContentPortal> request) =>
        await proxyWrappers.Send<IList<Style>>("StyleFetchForContentPortal", request);

    public async Task<Response<Style?>> Fetch(Request<Style> request) =>
        await proxyWrappers.Send<Style?>("StyleFetch", request);

    public async Task<Response> Save(Request<Style> request) =>
        await proxyWrappers.Send("StyleSave", request);

    public async Task<Response<IList<RuleFull>>> FetchRules(Request request) =>
        await proxyWrappers.SendAndCache<IList<RuleFull>>("StyleFetchRules", request);
}