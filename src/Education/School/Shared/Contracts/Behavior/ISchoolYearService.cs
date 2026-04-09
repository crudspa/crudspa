namespace Crudspa.Education.School.Shared.Contracts.Behavior;

public interface ISchoolYearService
{
    Task<Response<SchoolYear?>> FetchCurrent(Request request);
}