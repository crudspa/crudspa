namespace Crudspa.Content.Design.Client.Services;

public class FontServiceTcp(IProxyWrappers proxyWrappers) : IFontService
{
    public async Task<Response<IList<Font>>> FetchForContentPortal(Request<ContentPortal> request) =>
        await proxyWrappers.Send<IList<Font>>("FontFetchForContentPortal", request);

    public async Task<Response<Font?>> Fetch(Request<Font> request) =>
        await proxyWrappers.Send<Font?>("FontFetch", request);

    public async Task<Response<Font?>> Add(Request<Font> request) =>
        await proxyWrappers.Send<Font?>("FontAdd", request);

    public async Task<Response> Save(Request<Font> request) =>
        await proxyWrappers.Send("FontSave", request);

    public async Task<Response> Remove(Request<Font> request) =>
        await proxyWrappers.Send("FontRemove", request);

    public async Task<Response<IList<IconFull>>> FetchIcons(Request request) =>
        await proxyWrappers.SendAndCache<IList<IconFull>>("FontFetchIcons", request);
}