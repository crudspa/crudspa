namespace Crudspa.Education.Publisher.Server.Services;

public class TrackLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ITrackLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<TrackLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<TrackLicense>>(request, async response =>
        {
            var trackLicenses = await TrackLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return trackLicenses;
        });
    }

    public async Task<Response<TrackLicense?>> Fetch(Request<TrackLicense> request)
    {
        return await wrappers.Try<TrackLicense?>(request, async response =>
        {
            var trackLicense = await TrackLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return trackLicense;
        });
    }

    public async Task<Response<TrackLicense?>> Add(Request<TrackLicense> request)
    {
        return await wrappers.Validate<TrackLicense?, TrackLicense>(request, async response =>
        {
            var trackLicense = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await TrackLicenseInsert.Execute(connection, transaction, request.SessionId, trackLicense);

                return new TrackLicense
                {
                    Id = id,
                    LicenseId = trackLicense.LicenseId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<TrackLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var trackLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrackLicenseUpdate.Execute(connection, transaction, request.SessionId, trackLicense);
            });
        });
    }

    public async Task<Response> Remove(Request<TrackLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var trackLicense = request.Value;
            var existing = await TrackLicenseSelect.Execute(Connection, request.SessionId, trackLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await TrackLicenseDelete.Execute(connection, transaction, request.SessionId, trackLicense);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchTrackNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await Crudspa.Education.Publisher.Server.Sproxies.TrackSelectOrderables.Execute(Connection, request.SessionId));
    }
}