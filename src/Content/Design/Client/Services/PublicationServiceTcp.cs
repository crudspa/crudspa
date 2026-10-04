namespace Crudspa.Content.Design.Client.Services;

using Publication = Shared.Contracts.Data.Publication;

public class PublicationServiceTcp(IProxyWrappers proxyWrappers) : IPublicationService
{
    public async Task<Response<IList<Publication>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Publication>>("PublicationFetchForPortal", request);

    public async Task<Response<Publication?>> Fetch(Request<Publication> request) =>
        await proxyWrappers.Send<Publication?>("PublicationFetch", request);

    public async Task<Response<Publication?>> Add(Request<Publication> request) =>
        await proxyWrappers.Send<Publication?>("PublicationAdd", request);

    public async Task<Response> Save(Request<Publication> request) =>
        await proxyWrappers.Send("PublicationSave", request);

    public async Task<Response> Remove(Request<Publication> request) =>
        await proxyWrappers.Send("PublicationRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Publication>> request) =>
        await proxyWrappers.Send("PublicationSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchPublicationGroupNames(Request<Portal> request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PublicationFetchPublicationGroupNames", request);

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PublicationFetchContentStatusNames", request);

    public async Task<Response<IList<Orderable>>> FetchPublicationResourceTypeNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("PublicationFetchPublicationResourceTypeNames", request);
}