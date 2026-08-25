namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using License = Data.License;

public interface ITrackLicenseService
{
    Task<Response<IList<TrackLicense>>> FetchForLicense(Request<License> request);
    Task<Response<TrackLicense?>> Fetch(Request<TrackLicense> request);
    Task<Response<TrackLicense?>> Add(Request<TrackLicense> request);
    Task<Response> Save(Request<TrackLicense> request);
    Task<Response> Remove(Request<TrackLicense> request);
    Task<Response<IList<Orderable>>> FetchTrackNames(Request request);
}