namespace Crudspa.Content.Messaging.Server.Services;

public class StageServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IStageService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Stage>>> FetchForCampaign(Request<Campaign> request)
    {
        return await wrappers.Try<IList<Stage>>(request, async response =>
        {
            var stages = await StageSelectForCampaign.Execute(Connection, request.SessionId, request.Value.Id);

            return stages;
        });
    }

    public async Task<Response<Stage?>> Fetch(Request<Stage> request)
    {
        return await wrappers.Try<Stage?>(request, async response =>
        {
            var stage = await StageSelect.Execute(Connection, request.SessionId, request.Value);

            return stage;
        });
    }

    public async Task<Response<Stage?>> Add(Request<Stage> request)
    {
        return await wrappers.Validate<Stage?, Stage>(request, async response =>
        {
            var stage = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await StageInsert.Execute(connection, transaction, request.SessionId, stage);

                return new Stage
                {
                    Id = id,
                    CampaignId = stage.CampaignId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Stage> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var stage = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StageUpdate.Execute(connection, transaction, request.SessionId, stage);
            });
        });
    }

    public async Task<Response> Remove(Request<Stage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var stage = request.Value;
            var existing = await StageSelect.Execute(Connection, request.SessionId, stage);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StageDelete.Execute(connection, transaction, request.SessionId, stage);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Stage>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var stages = request.Value;

            stages.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StageUpdateOrdinals.Execute(connection, transaction, request.SessionId, stages);
            });
        });
    }
}