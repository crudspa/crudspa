using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Client.Services;

public class UnitLicenseServiceTcp(IProxyWrappers proxyWrappers) : IUnitLicenseService
{
    public async Task<Response<IList<UnitLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<UnitLicense>>("UnitLicenseFetchForLicense", request);

    public async Task<Response<UnitLicense?>> Fetch(Request<UnitLicense> request) =>
        await proxyWrappers.Send<UnitLicense?>("UnitLicenseFetch", request);

    public async Task<Response<UnitLicense?>> Add(Request<UnitLicense> request) =>
        await proxyWrappers.Send<UnitLicense?>("UnitLicenseAdd", request);

    public async Task<Response> Save(Request<UnitLicense> request) =>
        await proxyWrappers.Send("UnitLicenseSave", request);

    public async Task<Response> Remove(Request<UnitLicense> request) =>
        await proxyWrappers.Send("UnitLicenseRemove", request);

    public async Task<Response> SaveRelations(Request<UnitLicense> request) =>
        await proxyWrappers.Send("UnitLicenseSaveRelations", request);

    public async Task<Response<IList<Orderable>>> FetchUnitNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("UnitLicenseFetchUnitNames", request);
}