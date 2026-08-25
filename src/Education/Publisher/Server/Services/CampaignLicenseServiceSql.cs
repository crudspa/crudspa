namespace Crudspa.Education.Publisher.Server.Services;

public class CampaignLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ICampaignLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<CampaignLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<CampaignLicense>>(request, async response =>
        {
            var campaignLicenses = await CampaignLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return campaignLicenses;
        });
    }

    public async Task<Response<CampaignLicense?>> Fetch(Request<CampaignLicense> request)
    {
        return await wrappers.Try<CampaignLicense?>(request, async response =>
        {
            var campaignLicense = await CampaignLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return campaignLicense;
        });
    }

    public async Task<Response<CampaignLicense?>> Add(Request<CampaignLicense> request)
    {
        return await wrappers.Validate<CampaignLicense?, CampaignLicense>(request, async response =>
        {
            var campaignLicense = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await CampaignLicenseInsert.Execute(connection, transaction, request.SessionId, campaignLicense);

                return new CampaignLicense
                {
                    Id = id,
                    LicenseId = campaignLicense.LicenseId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<CampaignLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var campaignLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CampaignLicenseUpdate.Execute(connection, transaction, request.SessionId, campaignLicense);
            });
        });
    }

    public async Task<Response> Remove(Request<CampaignLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var campaignLicense = request.Value;
            var existing = await CampaignLicenseSelect.Execute(Connection, request.SessionId, campaignLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CampaignLicenseDelete.Execute(connection, transaction, request.SessionId, campaignLicense);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchCampaignNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await Crudspa.Education.Publisher.Server.Sproxies.CampaignSelectNames.Execute(Connection, request.SessionId));
    }
}