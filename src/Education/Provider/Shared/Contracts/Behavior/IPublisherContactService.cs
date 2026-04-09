namespace Crudspa.Education.Provider.Shared.Contracts.Behavior;

public interface IPublisherContactService
{
    Task<Response<IList<PublisherContact>>> SearchForPublisher(Request<PublisherContactSearch> request);
    Task<Response<PublisherContact?>> Fetch(Request<PublisherContact> request);
    Task<Response<PublisherContact?>> Add(Request<PublisherContact> request);
    Task<Response> Save(Request<PublisherContact> request);
    Task<Response> Remove(Request<PublisherContact> request);
    Task<Response<IList<Named>>> FetchRoleNames(Request<Publisher> request);
    Task<Response> SendAccessCode(Request<PublisherContact> request);
}