namespace Crudspa.Education.Publisher.Client.Services;

using License = Shared.Contracts.Data.License;

public class TrackLicenseServiceTcp(IProxyWrappers proxyWrappers) : ITrackLicenseService
{
    public async Task<Response<IList<TrackLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<TrackLicense>>("TrackLicenseFetchForLicense", request);

    public async Task<Response<TrackLicense?>> Fetch(Request<TrackLicense> request) =>
        await proxyWrappers.Send<TrackLicense?>("TrackLicenseFetch", request);

    public async Task<Response<TrackLicense?>> Add(Request<TrackLicense> request) =>
        await proxyWrappers.Send<TrackLicense?>("TrackLicenseAdd", request);

    public async Task<Response> Save(Request<TrackLicense> request) =>
        await proxyWrappers.Send("TrackLicenseSave", request);

    public async Task<Response> Remove(Request<TrackLicense> request) =>
        await proxyWrappers.Send("TrackLicenseRemove", request);

    public async Task<Response<IList<Orderable>>> FetchTrackNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("TrackLicenseFetchTrackNames", request);
}