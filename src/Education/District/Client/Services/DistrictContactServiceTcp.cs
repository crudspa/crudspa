namespace Crudspa.Education.District.Client.Services;

public class DistrictContactServiceTcp(IProxyWrappers proxyWrappers) : IDistrictContactService
{
    public async Task<Response<IList<DistrictContact>>> Search(Request<DistrictContactSearch> request) =>
        await proxyWrappers.Send<IList<DistrictContact>>("DistrictContactSearch", request);

    public async Task<Response<DistrictContact?>> Fetch(Request<DistrictContact> request) =>
        await proxyWrappers.Send<DistrictContact?>("DistrictContactFetch", request);

    public async Task<Response<DistrictContact?>> Add(Request<DistrictContact> request) =>
        await proxyWrappers.Send<DistrictContact?>("DistrictContactAdd", request);

    public async Task<Response> Save(Request<DistrictContact> request) =>
        await proxyWrappers.Send("DistrictContactSave", request);

    public async Task<Response> Remove(Request<DistrictContact> request) =>
        await proxyWrappers.Send("DistrictContactRemove", request);

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("DistrictContactFetchRoleNames", request);

    public async Task<Response> SendAccessCode(Request<DistrictContact> request) =>
        await proxyWrappers.Send("DistrictContactSendAccessCode", request);
}