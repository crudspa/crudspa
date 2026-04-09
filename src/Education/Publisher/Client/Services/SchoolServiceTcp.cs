namespace Crudspa.Education.Publisher.Client.Services;

public class SchoolServiceTcp(IProxyWrappers proxyWrappers) : ISchoolService
{
    public async Task<Response<IList<School>>> SearchForDistrict(Request<SchoolSearch> request) =>
        await proxyWrappers.Send<IList<School>>("SchoolSearchForDistrict", request);

    public async Task<Response<School?>> Fetch(Request<School> request) =>
        await proxyWrappers.Send<School?>("SchoolFetch", request);

    public async Task<Response<School?>> Add(Request<School> request) =>
        await proxyWrappers.Send<School?>("SchoolAdd", request);

    public async Task<Response> Save(Request<School> request) =>
        await proxyWrappers.Send("SchoolSave", request);

    public async Task<Response> Remove(Request<School> request) =>
        await proxyWrappers.Send("SchoolRemove", request);

    public async Task<Response<IList<Named>>> FetchCommunityNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("SchoolFetchCommunityNames", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("SchoolFetchPermissionNames", request);
}