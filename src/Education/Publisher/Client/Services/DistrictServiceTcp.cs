namespace Crudspa.Education.Publisher.Client.Services;

public class DistrictServiceTcp(IProxyWrappers proxyWrappers) : IDistrictService
{
    public async Task<Response<IList<District>>> Search(Request<DistrictSearch> request) =>
        await proxyWrappers.Send<IList<District>>("DistrictSearch", request);

    public async Task<Response<District?>> Fetch(Request<District> request) =>
        await proxyWrappers.Send<District?>("DistrictFetch", request);

    public async Task<Response<District?>> Add(Request<District> request) =>
        await proxyWrappers.Send<District?>("DistrictAdd", request);

    public async Task<Response> Save(Request<District> request) =>
        await proxyWrappers.Send("DistrictSave", request);

    public async Task<Response> Remove(Request<District> request) =>
        await proxyWrappers.Send("DistrictRemove", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("DistrictFetchPermissionNames", request);
}