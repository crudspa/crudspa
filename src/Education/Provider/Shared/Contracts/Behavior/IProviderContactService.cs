namespace Crudspa.Education.Provider.Shared.Contracts.Behavior;

public interface IProviderContactService
{
    Task<Response<IList<ProviderContact>>> Search(Request<ProviderContactSearch> request);
    Task<Response<ProviderContact?>> Fetch(Request<ProviderContact> request);
    Task<Response<ProviderContact?>> Add(Request<ProviderContact> request);
    Task<Response> Save(Request<ProviderContact> request);
    Task<Response> Remove(Request<ProviderContact> request);
    Task<Response<IList<Named>>> FetchRoleNames(Request request);
    Task<Response> SendAccessCode(Request<ProviderContact> request);
}