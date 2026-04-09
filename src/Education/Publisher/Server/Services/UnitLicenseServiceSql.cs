namespace Crudspa.Education.Publisher.Server.Services;

public class UnitLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionService sessionService)
    : IUnitLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<UnitLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<UnitLicense>>(request, async response =>
        {
            var unitLicenses = await UnitLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);
            return unitLicenses;
        });
    }

    public async Task<Response<UnitLicense?>> Fetch(Request<UnitLicense> request)
    {
        return await wrappers.Try<UnitLicense?>(request, async response =>
        {
            var unitLicense = await UnitLicenseSelect.Execute(Connection, request.SessionId, request.Value);
            return unitLicense;
        });
    }

    public async Task<Response<UnitLicense?>> Add(Request<UnitLicense> request)
    {
        return await wrappers.Validate<UnitLicense?, UnitLicense>(request, async response =>
        {
            var unitLicense = request.Value;

            var addedUnitLicense = await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await UnitLicenseInsert.Execute(connection, transaction, request.SessionId, unitLicense);

                return new UnitLicense
                {
                    Id = id,
                    LicenseId = unitLicense.LicenseId,
                };
            });

            await sessionService.InvalidateAll();

            return addedUnitLicense;
        });
    }

    public async Task<Response> Save(Request<UnitLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var unitLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitLicenseUpdate.Execute(connection, transaction, request.SessionId, unitLicense);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> Remove(Request<UnitLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var unitLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitLicenseDelete.Execute(connection, transaction, request.SessionId, unitLicense);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> SaveRelations(Request<UnitLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var unitLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitLicenseUpdateRelations.Execute(connection, transaction, request.SessionId, unitLicense);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response<IList<Orderable>>> FetchUnitNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await UnitSelectOrderables.Execute(Connection));
    }
}