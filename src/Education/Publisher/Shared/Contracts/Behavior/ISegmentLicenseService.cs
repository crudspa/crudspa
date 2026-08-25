namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using License = Data.License;

public interface ISegmentLicenseService
{
    Task<Response<IList<SegmentLicense>>> FetchForLicense(Request<License> request);
    Task<Response<SegmentLicense?>> Fetch(Request<SegmentLicense> request);
    Task<Response<SegmentLicense?>> Add(Request<SegmentLicense> request);
    Task<Response> Save(Request<SegmentLicense> request);
    Task<Response> Remove(Request<SegmentLicense> request);
    Task<Response<IList<Orderable>>> FetchSegmentNames(Request request);
}