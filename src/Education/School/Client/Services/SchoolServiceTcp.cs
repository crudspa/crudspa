namespace Crudspa.Education.School.Client.Services;

using School = Shared.Contracts.Data.School;

public class SchoolServiceTcp(IProxyWrappers proxyWrappers) : ISchoolService
{
    public async Task<Response<School?>> Fetch(Request request) =>
        await proxyWrappers.Send<School?>("SchoolFetch", request);

    public async Task<Response> Save(Request<School> request) =>
        await proxyWrappers.Send("SchoolSave", request);

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("SchoolFetchPermissionNames", request);
}