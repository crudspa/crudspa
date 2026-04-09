namespace Crudspa.Education.Publisher.Client.Services;

using Publisher = Shared.Contracts.Data.Publisher;

public class PublisherServiceTcp(IProxyWrappers proxyWrappers) : IPublisherService
{
    public async Task<Response<Publisher?>> Fetch(Request request) =>
        await proxyWrappers.Send<Publisher?>("PublisherFetch", request);

    public async Task<Response> Save(Request<Publisher> request) =>
        await proxyWrappers.Send("PublisherSave", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("PublisherFetchPermissionNames", request);
}