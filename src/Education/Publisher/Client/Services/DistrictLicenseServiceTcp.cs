using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Client.Services;

public class DistrictLicenseServiceTcp(IProxyWrappers proxyWrappers) : IDistrictLicenseService
{
    public async Task<Response<IList<DistrictLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<DistrictLicense>>("DistrictLicenseFetchForLicense", request);

    public async Task<Response<DistrictLicense?>> Fetch(Request<DistrictLicense> request) =>
        await proxyWrappers.Send<DistrictLicense?>("DistrictLicenseFetch", request);

    public async Task<Response<DistrictLicense?>> Add(Request<DistrictLicense> request) =>
        await proxyWrappers.Send<DistrictLicense?>("DistrictLicenseAdd", request);

    public async Task<Response> Save(Request<DistrictLicense> request) =>
        await proxyWrappers.Send("DistrictLicenseSave", request);

    public async Task<Response> Remove(Request<DistrictLicense> request) =>
        await proxyWrappers.Send("DistrictLicenseRemove", request);

    public async Task<Response> SaveRelations(Request<DistrictLicense> request) =>
        await proxyWrappers.Send("DistrictLicenseSaveRelations", request);

    public async Task<Response<IList<Named>>> FetchDistrictNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("DistrictLicenseFetchDistrictNames", request);
}