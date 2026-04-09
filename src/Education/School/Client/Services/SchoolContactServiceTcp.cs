namespace Crudspa.Education.School.Client.Services;

public class SchoolContactServiceTcp(IProxyWrappers proxyWrappers) : ISchoolContactService
{
    public async Task<Response<IList<SchoolContact>>> Search(Request<SchoolContactSearch> request) =>
        await proxyWrappers.Send<IList<SchoolContact>>("SchoolContactSearch", request);

    public async Task<Response<SchoolContact?>> Fetch(Request<SchoolContact> request) =>
        await proxyWrappers.Send<SchoolContact?>("SchoolContactFetch", request);

    public async Task<Response<SchoolContact?>> Add(Request<SchoolContact> request) =>
        await proxyWrappers.Send<SchoolContact?>("SchoolContactAdd", request);

    public async Task<Response> Save(Request<SchoolContact> request) =>
        await proxyWrappers.Send("SchoolContactSave", request);

    public async Task<Response> Remove(Request<SchoolContact> request) =>
        await proxyWrappers.Send("SchoolContactRemove", request);

    public async Task<Response<IList<Orderable>>> FetchTitleNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("SchoolContactFetchTitleNames", request);

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("SchoolContactFetchRoleNames", request);

    public async Task<Response> SendAccessCode(Request<SchoolContact> request) =>
        await proxyWrappers.Send("SchoolContactSendAccessCode", request);

    public async Task<Response<SchoolContact>> FetchRelations(Request<SchoolContact> request) =>
        await proxyWrappers.Send<SchoolContact>("SchoolContactFetchRelations", request);

    public async Task<Response> SaveRelations(Request<SchoolContact> request) =>
        await proxyWrappers.Send("SchoolContactSaveRelations", request);
}