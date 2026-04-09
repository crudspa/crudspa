namespace Crudspa.Education.Provider.Shared.Contracts.Behavior;

using Provider = Data.Provider;

public interface IProviderService
{
    Task<Response<Provider?>> Fetch(Request request);
    Task<Response> Save(Request<Provider> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}