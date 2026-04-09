namespace Crudspa.Education.School.Client.Services;

public class ClassroomServiceTcp(IProxyWrappers proxyWrappers) : IClassroomService
{
    public async Task<Response<IList<Classroom>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<Classroom>>("ClassroomFetchAll", request);

    public async Task<Response<Classroom?>> Fetch(Request<Classroom> request) =>
        await proxyWrappers.Send<Classroom?>("ClassroomFetch", request);

    public async Task<Response<Classroom?>> Add(Request<Classroom> request) =>
        await proxyWrappers.Send<Classroom?>("ClassroomAdd", request);

    public async Task<Response> Save(Request<Classroom> request) =>
        await proxyWrappers.Send("ClassroomSave", request);

    public async Task<Response> Remove(Request<Classroom> request) =>
        await proxyWrappers.Send("ClassroomRemove", request);

    public async Task<Response<IList<Selectable>>> FetchStudents(Request<Classroom> request) =>
        await proxyWrappers.Send<IList<Selectable>>("ClassroomFetchStudents", request);

    public async Task<Response<IList<Selectable>>> FetchSchoolContacts(Request<Classroom> request) =>
        await proxyWrappers.Send<IList<Selectable>>("ClassroomFetchSchoolContacts", request);

    public async Task<Response<IList<Orderable>>> FetchTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ClassroomFetchClassroomTypes", request);

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ClassroomFetchClassroomGrades", request);

    public async Task<Response<IList<Named>>> FetchClassroomNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("ClassroomFetchClassroomNames", request);

    public async Task<Response<District?>> FetchDistrict(Request request) =>
        await proxyWrappers.Send<District?>("ClassroomFetchDistrict", request);
}