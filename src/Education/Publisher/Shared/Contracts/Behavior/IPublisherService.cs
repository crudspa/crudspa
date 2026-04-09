namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using Publisher = Data.Publisher;

public interface IPublisherService
{
    Task<Response<Publisher?>> Fetch(Request request);
    Task<Response> Save(Request<Publisher> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}