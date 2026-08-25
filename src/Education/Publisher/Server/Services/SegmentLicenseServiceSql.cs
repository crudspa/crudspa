namespace Crudspa.Education.Publisher.Server.Services;

public class SegmentLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ISegmentLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SegmentLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<SegmentLicense>>(request, async response =>
        {
            var segmentLicenses = await SegmentLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return segmentLicenses;
        });
    }

    public async Task<Response<SegmentLicense?>> Fetch(Request<SegmentLicense> request)
    {
        return await wrappers.Try<SegmentLicense?>(request, async response =>
        {
            var segmentLicense = await SegmentLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return segmentLicense;
        });
    }

    public async Task<Response<SegmentLicense?>> Add(Request<SegmentLicense> request)
    {
        return await wrappers.Validate<SegmentLicense?, SegmentLicense>(request, async response =>
        {
            var segmentLicense = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await SegmentLicenseInsert.Execute(connection, transaction, request.SessionId, segmentLicense);

                return new SegmentLicense
                {
                    Id = id,
                    LicenseId = segmentLicense.LicenseId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<SegmentLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var segmentLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SegmentLicenseUpdate.Execute(connection, transaction, request.SessionId, segmentLicense);
            });
        });
    }

    public async Task<Response> Remove(Request<SegmentLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var segmentLicense = request.Value;
            var existing = await SegmentLicenseSelect.Execute(Connection, request.SessionId, segmentLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SegmentLicenseDelete.Execute(connection, transaction, request.SessionId, segmentLicense);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchSegmentNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await Crudspa.Education.Publisher.Server.Sproxies.SegmentSelectOrderables.Execute(Connection, request.SessionId));
    }
}