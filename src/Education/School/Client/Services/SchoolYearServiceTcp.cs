namespace Crudspa.Education.School.Client.Services;

public class SchoolYearServiceTcp(IProxyWrappers proxyWrappers) : ISchoolYearService
{
    public async Task<Response<IList<SchoolYear>>> FetchAll(Request request) =>
        await proxyWrappers.SendAndCache<IList<SchoolYear>>("SchoolYearFetchAll", request);

    public async Task<Response<SchoolYear?>> FetchCurrent(Request request) =>
        await proxyWrappers.SendAndCache<SchoolYear?>("SchoolYearFetchCurrent", request);

    public async Task<Response> SaveAll(Request<IList<SchoolYear>> request) =>
        await proxyWrappers.Send("SchoolYearSaveAll", request);
}