namespace Crudspa.Education.Publisher.Server.Services;

public class DistrictLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionService sessionService)
    : IDistrictLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<DistrictLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<DistrictLicense>>(request, async response =>
        {
            var districtLicenses = await DistrictLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return districtLicenses;
        });
    }

    public async Task<Response<DistrictLicense?>> Fetch(Request<DistrictLicense> request)
    {
        return await wrappers.Try<DistrictLicense?>(request, async response =>
        {
            var districtLicense = await DistrictLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return districtLicense;
        });
    }

    public async Task<Response<DistrictLicense?>> Add(Request<DistrictLicense> request)
    {
        return await wrappers.Validate<DistrictLicense?, DistrictLicense>(request, async response =>
        {
            var districtLicense = request.Value;

            var addedDistrictLicense = await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await DistrictLicenseInsert.Execute(connection, transaction, request.SessionId, districtLicense);

                return new DistrictLicense
                {
                    Id = id,
                    LicenseId = districtLicense.LicenseId,
                };
            });

            await sessionService.InvalidateAll();

            return addedDistrictLicense;
        });
    }

    public async Task<Response> Save(Request<DistrictLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var districtLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await DistrictLicenseUpdate.Execute(connection, transaction, request.SessionId, districtLicense);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> Remove(Request<DistrictLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var districtLicense = request.Value;
            var existing = await DistrictLicenseSelect.Execute(Connection, request.SessionId, districtLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await DistrictLicenseDelete.Execute(connection, transaction, request.SessionId, districtLicense);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> SaveRelations(Request<DistrictLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var districtLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await DistrictLicenseUpdateRelations.Execute(connection, transaction, request.SessionId, districtLicense);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response<IList<Named>>> FetchDistrictNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await DistrictSelectNames.Execute(Connection, request.SessionId));
    }
}