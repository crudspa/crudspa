namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface ISchoolYearService
{
    Task<Response<IList<SchoolYear>>> FetchAll(Request request);
    Task<Response<SchoolYear?>> Fetch(Request<SchoolYear> request);
    Task<Response<SchoolYear?>> Add(Request<SchoolYear> request);
    Task<Response> Save(Request<SchoolYear> request);
    Task<Response> Remove(Request<SchoolYear> request);
}