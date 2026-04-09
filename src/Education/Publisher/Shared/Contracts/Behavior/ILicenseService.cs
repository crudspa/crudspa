using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface ILicenseService
{
    Task<Response<IList<License>>> Search(Request<LicenseSearch> request);
    Task<Response<License?>> Fetch(Request<License> request);
    Task<Response<License?>> Add(Request<License> request);
    Task<Response> Save(Request<License> request);
    Task<Response> Remove(Request<License> request);
}