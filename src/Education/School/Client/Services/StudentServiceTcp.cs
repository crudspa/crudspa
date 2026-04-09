namespace Crudspa.Education.School.Client.Services;

public class StudentServiceTcp(IProxyWrappers proxyWrappers) : IStudentService
{
    public async Task<Response<IList<Student>>> Search(Request<StudentSearch> request) =>
        await proxyWrappers.Send<IList<Student>>("StudentSearch", request);

    public async Task<Response<Student?>> Fetch(Request<Student> request) =>
        await proxyWrappers.Send<Student?>("StudentFetch", request);

    public async Task<Response<Student?>> Add(Request<Student> request) =>
        await proxyWrappers.Send<Student?>("StudentAdd", request);

    public async Task<Response> Save(Request<Student> request) =>
        await proxyWrappers.Send("StudentSave", request);

    public async Task<Response> Remove(Request<Student> request) =>
        await proxyWrappers.Send("StudentRemove", request);

    public async Task<Response<IList<Orderable>>> FetchGrades(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("StudentFetchGrades", request);

    public async Task<Response<IList<Orderable>>> FetchAssessmentTypes(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("StudentFetchAssessmentTypes", request);

    public async Task<Response<IList<Orderable>>> FetchAssessmentLevels(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("StudentFetchAssessmentLevels", request);

    public async Task<Response<IList<Named>>> FetchSchoolYears(Request request) =>
        await proxyWrappers.SendAndCache<IList<Named>>("StudentFetchSchoolYears", request);

    public async Task<Response<Student>> GenerateSecretCode(Request request) =>
        await proxyWrappers.Send<Student>("StudentGenerateSecretCode", request);

    public async Task<Response<District?>> FetchDistrict(Request request) =>
        await proxyWrappers.Send<District?>("StudentFetchDistrict", request);

    public async Task<Response<IList<Named>>> FetchSchools(Request request) =>
        await proxyWrappers.Send<IList<Named>>("StudentFetchSchools", request);

    public async Task<Response<IList<Selectable>>> FetchSelectableClassrooms(Request<StudentClassroomSearch> request) =>
        await proxyWrappers.Send<IList<Selectable>>("StudentFetchSelectableClassrooms", request);
}