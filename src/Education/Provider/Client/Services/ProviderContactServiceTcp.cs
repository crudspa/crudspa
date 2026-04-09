namespace Crudspa.Education.Provider.Client.Services;

public class ProviderContactServiceTcp(IProxyWrappers proxyWrappers) : IProviderContactService
{
    public async Task<Response<IList<ProviderContact>>> Search(Request<ProviderContactSearch> request) =>
        await proxyWrappers.Send<IList<ProviderContact>>("ProviderContactSearch", request);

    public async Task<Response<ProviderContact?>> Fetch(Request<ProviderContact> request) =>
        await proxyWrappers.Send<ProviderContact?>("ProviderContactFetch", request);

    public async Task<Response<ProviderContact?>> Add(Request<ProviderContact> request) =>
        await proxyWrappers.Send<ProviderContact?>("ProviderContactAdd", request);

    public async Task<Response> Save(Request<ProviderContact> request) =>
        await proxyWrappers.Send("ProviderContactSave", request);

    public async Task<Response> Remove(Request<ProviderContact> request) =>
        await proxyWrappers.Send("ProviderContactRemove", request);

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("ProviderContactFetchRoleNames", request);

    public async Task<Response> SendAccessCode(Request<ProviderContact> request) =>
        await proxyWrappers.Send("ProviderContactSendAccessCode", request);
}