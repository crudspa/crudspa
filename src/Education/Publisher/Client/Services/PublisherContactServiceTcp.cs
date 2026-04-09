namespace Crudspa.Education.Publisher.Client.Services;

public class PublisherContactServiceTcp(IProxyWrappers proxyWrappers) : IPublisherContactService
{
    public async Task<Response<IList<PublisherContact>>> Search(Request<PublisherContactSearch> request) =>
        await proxyWrappers.Send<IList<PublisherContact>>("PublisherContactSearch", request);

    public async Task<Response<PublisherContact?>> Fetch(Request<PublisherContact> request) =>
        await proxyWrappers.Send<PublisherContact?>("PublisherContactFetch", request);

    public async Task<Response<PublisherContact?>> Add(Request<PublisherContact> request) =>
        await proxyWrappers.Send<PublisherContact?>("PublisherContactAdd", request);

    public async Task<Response> Save(Request<PublisherContact> request) =>
        await proxyWrappers.Send("PublisherContactSave", request);

    public async Task<Response> Remove(Request<PublisherContact> request) =>
        await proxyWrappers.Send("PublisherContactRemove", request);

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("PublisherContactFetchRoleNames", request);

    public async Task<Response> SendAccessCode(Request<PublisherContact> request) =>
        await proxyWrappers.Send("PublisherContactSendAccessCode", request);
}