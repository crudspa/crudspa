namespace Crudspa.Education.School.Server.Services;

public class SchoolYearServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ISchoolYearService
{
    private readonly ISqlWrappers _sqlWrappers = sqlWrappers;

    private String Connection => configService.Fetch().Database;

    public async Task<Response<SchoolYear?>> FetchCurrent(Request request)
    {
        return await wrappers.Try<SchoolYear?>(request, async response =>
        {
            var schoolYear = await SchoolYearSelectCurrent.Execute(Connection);
            return schoolYear;
        });
    }
}