namespace Crudspa.Education.Provider.Shared.Contracts.Behavior;

public interface IPublisherService
{
    Task<Response<IList<Publisher>>> Search(Request<PublisherSearch> request);
    Task<Response<Publisher?>> Fetch(Request<Publisher> request);
    Task<Response<Publisher?>> Add(Request<Publisher> request);
    Task<Response> Save(Request<Publisher> request);
    Task<Response> Remove(Request<Publisher> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}