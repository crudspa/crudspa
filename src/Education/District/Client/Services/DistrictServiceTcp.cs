namespace Crudspa.Education.District.Client.Services;

using District = Shared.Contracts.Data.District;

public class DistrictServiceTcp(IProxyWrappers proxyWrappers) : IDistrictService
{
    public async Task<Response<District?>> Fetch(Request request) =>
        await proxyWrappers.Send<District?>("DistrictFetch", request);

    public async Task<Response> Save(Request<District> request) =>
        await proxyWrappers.Send("DistrictSave", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("DistrictFetchPermissionNames", request);

    public async Task<Response<IList<Named>>> FetchCommunityNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("DistrictFetchCommunityNames", request);
}