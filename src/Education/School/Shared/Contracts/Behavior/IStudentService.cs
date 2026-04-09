namespace Crudspa.Education.School.Shared.Contracts.Behavior;

public interface IStudentService
{
    Task<Response<IList<Student>>> Search(Request<StudentSearch> request);
    Task<Response<Student?>> Fetch(Request<Student> request);
    Task<Response<Student?>> Add(Request<Student> request);
    Task<Response> Save(Request<Student> request);
    Task<Response> Remove(Request<Student> request);
    Task<Response<IList<Orderable>>> FetchGrades(Request request);
    Task<Response<IList<Orderable>>> FetchAssessmentTypes(Request request);
    Task<Response<IList<Orderable>>> FetchAssessmentLevels(Request request);
    Task<Response<IList<Named>>> FetchSchoolYears(Request request);
    Task<Response<Student>> GenerateSecretCode(Request request);
    Task<Response<District?>> FetchDistrict(Request request);
    Task<Response<IList<Named>>> FetchSchools(Request request);
    Task<Response<IList<Selectable>>> FetchSelectableClassrooms(Request<StudentClassroomSearch> request);
}