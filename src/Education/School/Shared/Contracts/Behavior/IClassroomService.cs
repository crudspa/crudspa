namespace Crudspa.Education.School.Shared.Contracts.Behavior;

public interface IClassroomService
{
    Task<Response<IList<Classroom>>> FetchAll(Request request);
    Task<Response<Classroom?>> Fetch(Request<Classroom> request);
    Task<Response<Classroom?>> Add(Request<Classroom> request);
    Task<Response> Save(Request<Classroom> request);
    Task<Response> Remove(Request<Classroom> request);
    Task<Response<IList<Selectable>>> FetchStudents(Request<Classroom> request);
    Task<Response<IList<Selectable>>> FetchSchoolContacts(Request<Classroom> request);
    Task<Response<IList<Orderable>>> FetchTypeNames(Request request);
    Task<Response<IList<Orderable>>> FetchGradeNames(Request request);
    Task<Response<IList<Named>>> FetchClassroomNames(Request request);
    Task<Response<District?>> FetchDistrict(Request request);
}