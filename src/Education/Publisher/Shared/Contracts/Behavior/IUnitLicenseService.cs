using License = Crudspa.Education.Publisher.Shared.Contracts.Data.License;

namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IUnitLicenseService
{
    Task<Response<IList<UnitLicense>>> FetchForLicense(Request<License> request);
    Task<Response<UnitLicense?>> Fetch(Request<UnitLicense> request);
    Task<Response<UnitLicense?>> Add(Request<UnitLicense> request);
    Task<Response> Save(Request<UnitLicense> request);
    Task<Response> Remove(Request<UnitLicense> request);
    Task<Response> SaveRelations(Request<UnitLicense> request);
    Task<Response<IList<Orderable>>> FetchUnitNames(Request request);
}