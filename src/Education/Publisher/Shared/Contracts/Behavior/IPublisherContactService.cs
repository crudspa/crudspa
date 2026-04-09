namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IPublisherContactService
{
    Task<Response<IList<PublisherContact>>> Search(Request<PublisherContactSearch> request);
    Task<Response<PublisherContact?>> Fetch(Request<PublisherContact> request);
    Task<Response<PublisherContact?>> Add(Request<PublisherContact> request);
    Task<Response> Save(Request<PublisherContact> request);
    Task<Response> Remove(Request<PublisherContact> request);
    Task<Response<IList<Named>>> FetchRoleNames(Request request);
    Task<Response> SendAccessCode(Request<PublisherContact> request);
}