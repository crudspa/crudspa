using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IDistrictLicenseService
{
    Task<Response<IList<DistrictLicense>>> FetchForLicense(Request<License> request);
    Task<Response<DistrictLicense?>> Fetch(Request<DistrictLicense> request);
    Task<Response<DistrictLicense?>> Add(Request<DistrictLicense> request);
    Task<Response> Save(Request<DistrictLicense> request);
    Task<Response> Remove(Request<DistrictLicense> request);
    Task<Response> SaveRelations(Request<DistrictLicense> request);
    Task<Response<IList<Named>>> FetchDistrictNames(Request request);
}