namespace Crudspa.Education.Publisher.Server.Services;

public class SchoolYearServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ISchoolYearService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SchoolYear>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<SchoolYear>>(request, async response =>
        {
            var schoolYears = await SchoolYearSelectAll.Execute(Connection);
            return schoolYears;
        });
    }

    public async Task<Response<SchoolYear?>> Fetch(Request<SchoolYear> request)
    {
        return await wrappers.Try<SchoolYear?>(request, async response =>
        {
            var schoolYear = await SchoolYearSelect.Execute(Connection, request.Value);
            return schoolYear;
        });
    }

    public async Task<Response<SchoolYear?>> Add(Request<SchoolYear> request)
    {
        return await wrappers.Validate<SchoolYear?, SchoolYear>(request, async response =>
        {
            var schoolYear = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await SchoolYearInsert.Execute(connection, transaction, request.SessionId, schoolYear);

                return new SchoolYear
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<SchoolYear> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var schoolYear = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SchoolYearUpdate.Execute(connection, transaction, request.SessionId, schoolYear);
            });
        });
    }

    public async Task<Response> Remove(Request<SchoolYear> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var schoolYear = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SchoolYearDelete.Execute(connection, transaction, request.SessionId, schoolYear);
            });
        });
    }
}