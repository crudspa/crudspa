namespace Crudspa.Education.Provider.Client.Services;

using Provider = Shared.Contracts.Data.Provider;

public class ProviderServiceTcp(IProxyWrappers proxyWrappers) : IProviderService
{
    public async Task<Response<Provider?>> Fetch(Request request) =>
        await proxyWrappers.Send<Provider?>("ProviderFetch", request);

    public async Task<Response> Save(Request<Provider> request) =>
        await proxyWrappers.Send("ProviderSave", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("ProviderFetchPermissionNames", request);
}