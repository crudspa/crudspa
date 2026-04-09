namespace Crudspa.Education.Provider.Client.Services;

public class PublisherServiceTcp(IProxyWrappers proxyWrappers) : IPublisherService
{
    public async Task<Response<IList<Publisher>>> Search(Request<PublisherSearch> request) =>
        await proxyWrappers.Send<IList<Publisher>>("PublisherSearch", request);

    public async Task<Response<Publisher?>> Fetch(Request<Publisher> request) =>
        await proxyWrappers.Send<Publisher?>("PublisherFetch", request);

    public async Task<Response<Publisher?>> Add(Request<Publisher> request) =>
        await proxyWrappers.Send<Publisher?>("PublisherAdd", request);

    public async Task<Response> Save(Request<Publisher> request) =>
        await proxyWrappers.Send("PublisherSave", request);

    public async Task<Response> Remove(Request<Publisher> request) =>
        await proxyWrappers.Send("PublisherRemove", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("PublisherFetchPermissionNames", request);
}