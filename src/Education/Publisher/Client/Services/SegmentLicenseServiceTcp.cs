namespace Crudspa.Education.Publisher.Client.Services;

using License = Shared.Contracts.Data.License;

public class SegmentLicenseServiceTcp(IProxyWrappers proxyWrappers) : ISegmentLicenseService
{
    public async Task<Response<IList<SegmentLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<SegmentLicense>>("SegmentLicenseFetchForLicense", request);

    public async Task<Response<SegmentLicense?>> Fetch(Request<SegmentLicense> request) =>
        await proxyWrappers.Send<SegmentLicense?>("SegmentLicenseFetch", request);

    public async Task<Response<SegmentLicense?>> Add(Request<SegmentLicense> request) =>
        await proxyWrappers.Send<SegmentLicense?>("SegmentLicenseAdd", request);

    public async Task<Response> Save(Request<SegmentLicense> request) =>
        await proxyWrappers.Send("SegmentLicenseSave", request);

    public async Task<Response> Remove(Request<SegmentLicense> request) =>
        await proxyWrappers.Send("SegmentLicenseRemove", request);

    public async Task<Response<IList<Orderable>>> FetchSegmentNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("SegmentLicenseFetchSegmentNames", request);
}