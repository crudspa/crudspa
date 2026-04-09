namespace Crudspa.Education.Publisher.Client.Services;

public class SchoolYearServiceTcp(IProxyWrappers proxyWrappers) : ISchoolYearService
{
    public async Task<Response<IList<SchoolYear>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<SchoolYear>>("SchoolYearFetchAll", request);

    public async Task<Response<SchoolYear?>> Fetch(Request<SchoolYear> request) =>
        await proxyWrappers.Send<SchoolYear?>("SchoolYearFetch", request);

    public async Task<Response<SchoolYear?>> Add(Request<SchoolYear> request) =>
        await proxyWrappers.Send<SchoolYear?>("SchoolYearAdd", request);

    public async Task<Response> Save(Request<SchoolYear> request) =>
        await proxyWrappers.Send("SchoolYearSave", request);

    public async Task<Response> Remove(Request<SchoolYear> request) =>
        await proxyWrappers.Send("SchoolYearRemove", request);
}