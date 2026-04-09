namespace Crudspa.Education.Common.Server.Services;

public class ReportServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IReportService
{
    private readonly ISqlWrappers _sqlWrappers = sqlWrappers;

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Report>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<Report>>(request, async response =>
        {
            var reports = await ReportSelectAll.Execute(Connection, request.SessionId);
            return reports;
        });
    }

    public async Task<Response<Report?>> Fetch(Request<Report> request)
    {
        return await wrappers.Try<Report?>(request, async response =>
        {
            var report = await ReportSelect.Execute(Connection, request.SessionId, request.Value);
            return report;
        });
    }

    public async Task<Response<IList<Named>>> FetchReportNames(Request<Portal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ReportSelectNames.Execute(Connection, request.SessionId, request.Value.Id));
    }
}