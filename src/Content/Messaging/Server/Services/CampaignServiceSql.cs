namespace Crudspa.Content.Messaging.Server.Services;

public class CampaignServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ICampaignService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Campaign>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Campaign>>(request, async response =>
        {
            var campaigns = await CampaignSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return campaigns;
        });
    }

    public async Task<Response<Campaign?>> Fetch(Request<Campaign> request)
    {
        return await wrappers.Try<Campaign?>(request, async response =>
        {
            var campaign = await CampaignSelect.Execute(Connection, request.SessionId, request.Value);

            return campaign;
        });
    }

    public async Task<Response<IList<Named>>> FetchLicenseNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await LicenseSelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<Campaign?>> Add(Request<Campaign> request)
    {
        return await wrappers.Validate<Campaign?, Campaign>(request, async response =>
        {
            var campaign = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await CampaignInsert.Execute(connection, transaction, request.SessionId, campaign);

                return new Campaign
                {
                    Id = id,
                    PortalId = campaign.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Campaign> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var campaign = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CampaignUpdate.Execute(connection, transaction, request.SessionId, campaign);
            });
        });
    }

    public async Task<Response> Remove(Request<Campaign> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var campaign = request.Value;
            var existing = await CampaignSelect.Execute(Connection, request.SessionId, campaign);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CampaignDelete.Execute(connection, transaction, request.SessionId, campaign);
            });
        });
    }
}