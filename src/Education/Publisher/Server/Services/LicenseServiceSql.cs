namespace Crudspa.Education.Publisher.Server.Services;

public class LicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer,
    ISessionService sessionService)
    : ILicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<License>>> Search(Request<LicenseSearch> request)
    {
        return await wrappers.Try<IList<License>>(request, async response =>
        {
            return await LicenseSelectWhere.Execute(Connection, request.SessionId, request.Value);
        });
    }

    public async Task<Response<License?>> Fetch(Request<License> request)
    {
        return await wrappers.Try<License?>(request, async response =>
        {
            var license = await LicenseSelect.Execute(Connection, request.SessionId, request.Value);
            return license;
        });
    }

    public async Task<Response<License?>> Add(Request<License> request)
    {
        return await wrappers.Validate<License?, License>(request, async response =>
        {
            var license = request.Value;

            license.Description = htmlSanitizer.Sanitize(license.Description);

            var addedLicense = await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await LicenseInsert.Execute(connection, transaction, request.SessionId, license);

                return new License
                {
                    Id = id,
                };
            });

            await sessionService.InvalidateAll();

            return addedLicense;
        });
    }

    public async Task<Response> Save(Request<License> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var license = request.Value;

            license.Description = htmlSanitizer.Sanitize(license.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await LicenseUpdate.Execute(connection, transaction, request.SessionId, license);
            });

            await sessionService.InvalidateAll();
        });
    }

    public async Task<Response> Remove(Request<License> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var license = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await LicenseDelete.Execute(connection, transaction, request.SessionId, license);
            });

            await sessionService.InvalidateAll();
        });
    }
}