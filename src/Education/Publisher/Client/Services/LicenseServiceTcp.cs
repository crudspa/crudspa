using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Client.Services;

public class LicenseServiceTcp(IProxyWrappers proxyWrappers) : ILicenseService
{
    public async Task<Response<IList<License>>> Search(Request<LicenseSearch> request) =>
        await proxyWrappers.Send<IList<License>>("LicenseSearch", request);

    public async Task<Response<License?>> Fetch(Request<License> request) =>
        await proxyWrappers.Send<License?>("LicenseFetch", request);

    public async Task<Response<License?>> Add(Request<License> request) =>
        await proxyWrappers.Send<License?>("LicenseAdd", request);

    public async Task<Response> Save(Request<License> request) =>
        await proxyWrappers.Send("LicenseSave", request);

    public async Task<Response> Remove(Request<License> request) =>
        await proxyWrappers.Send("LicenseRemove", request);
}