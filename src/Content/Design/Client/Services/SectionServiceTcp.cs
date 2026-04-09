namespace Crudspa.Content.Design.Client.Services;

public class SectionServiceTcp(IProxyWrappers proxyWrappers) : ISectionService
{
    public async Task<Response<IList<Section>>> FetchForPage(Request<Page> request) =>
        await proxyWrappers.Send<IList<Section>>("SectionFetchForPage", request);

    public async Task<Response<Section?>> Fetch(Request<Section> request) =>
        await proxyWrappers.Send<Section?>("SectionFetch", request);

    public async Task<Response<Section?>> Add(Request<Section> request) =>
        await proxyWrappers.Send<Section?>("SectionAdd", request);

    public async Task<Response> Save(Request<Section> request) =>
        await proxyWrappers.Send("SectionSave", request);

    public async Task<Response> Remove(Request<Section> request) =>
        await proxyWrappers.Send("SectionRemove", request);

    public async Task<Response<Copy>> Copy(Request<Copy> request) =>
        await proxyWrappers.Send<Copy>("SectionCopy", request);

    public async Task<Response> SaveOrder(Request<IList<Section>> request) =>
        await proxyWrappers.Send("SectionSaveOrder", request);

    public async Task<Response<IList<ElementType>>> FetchElementTypes(Request request) =>
        await proxyWrappers.SendAndCache<IList<ElementType>>("SectionFetchElementTypes", request);

    public async Task<Response<SectionElement?>> CreateElement(Request<ElementSpec> request) =>
        await proxyWrappers.Send<SectionElement?>("SectionCreateElement", request);
}