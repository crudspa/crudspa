namespace Crudspa.Education.Common.Client.Services;

public class ReportServiceTcp(IProxyWrappers proxyWrappers) : IReportService
{
    public async Task<Response<IList<Report>>> FetchAll(Request request) =>
        await proxyWrappers.Send<IList<Report>>("ReportFetchAll", request);

    public async Task<Response<Report?>> Fetch(Request<Report> request) =>
        await proxyWrappers.Send<Report?>("ReportFetch", request);

    public async Task<Response<IList<Named>>> FetchReportNames(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Named>>("ReportFetchReportNames", request);
}