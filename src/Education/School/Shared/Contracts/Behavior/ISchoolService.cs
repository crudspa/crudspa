namespace Crudspa.Education.School.Shared.Contracts.Behavior;

using School = Data.School;

public interface ISchoolService
{
    Task<Response<School?>> Fetch(Request request);
    Task<Response> Save(Request<School> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}