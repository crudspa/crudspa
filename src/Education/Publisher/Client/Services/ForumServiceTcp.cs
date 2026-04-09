using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;
using IForumService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IForumService;

namespace Crudspa.Education.Publisher.Client.Services;

public class ForumServiceTcp(IProxyWrappers proxyWrappers) : IForumService
{
    public async Task<Response<IList<Forum>>> Search(Request<ForumSearch> request) =>
        await proxyWrappers.Send<IList<Forum>>("PublisherForumSearch", request);

    public async Task<Response<Forum?>> Fetch(Request<Forum> request) =>
        await proxyWrappers.Send<Forum?>("PublisherForumFetch", request);

    public async Task<Response<Forum?>> Add(Request<Forum> request) =>
        await proxyWrappers.Send<Forum?>("PublisherForumAdd", request);

    public async Task<Response> Save(Request<Forum> request) =>
        await proxyWrappers.Send("PublisherForumSave", request);

    public async Task<Response> Remove(Request<Forum> request) =>
        await proxyWrappers.Send("PublisherForumRemove", request);

    public async Task<Response<IList<Orderable>>> FetchBodyTemplateNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("PublisherForumFetchBodyTemplateNames", request);

    public async Task<Response<IList<Named>>> FetchDistrictNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("PublisherForumFetchDistrictNames", request);

    public async Task<Response<IList<Named>>> FetchSchoolNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("PublisherForumFetchSchoolNames", request);
}