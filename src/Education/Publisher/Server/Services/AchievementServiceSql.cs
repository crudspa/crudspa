using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;
using AchievementDelete = Crudspa.Education.Publisher.Server.Sproxies.AchievementDelete;
using AchievementInsert = Crudspa.Education.Publisher.Server.Sproxies.AchievementInsert;
using AchievementSelect = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelect;
using AchievementUpdate = Crudspa.Education.Publisher.Server.Sproxies.AchievementUpdate;
using IAchievementService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IAchievementService;

namespace Crudspa.Education.Publisher.Server.Services;

public class AchievementServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : IAchievementService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Achievement>>> Search(Request<AchievementSearch> request)
    {
        return await wrappers.Try<IList<Achievement>>(request, async response =>
        {
            return await AchievementSelectWhere.Execute(Connection, request.SessionId, request.Value);
        });
    }

    public async Task<Response<Achievement?>> Fetch(Request<Achievement> request)
    {
        return await wrappers.Try<Achievement?>(request, async response =>
        {
            var achievement = await AchievementSelect.Execute(Connection, request.SessionId, request.Value);
            return achievement;
        });
    }

    public async Task<Response<Achievement?>> Add(Request<Achievement> request)
    {
        return await wrappers.Validate<Achievement?, Achievement>(request, async response =>
        {
            var achievement = request.Value;

            var trophyImageFileResponse = await fileService.SaveImage(new(request.SessionId, achievement.TrophyImageFile));
            if (!trophyImageFileResponse.Ok)
            {
                response.AddErrors(trophyImageFileResponse.Errors);
                return null;
            }

            achievement.TrophyImageFile.Id = trophyImageFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await AchievementInsert.Execute(connection, transaction, request.SessionId, achievement);

                return new Achievement
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Achievement> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var achievement = request.Value;

            var existing = await AchievementSelect.Execute(Connection, request.SessionId, achievement);

            var trophyImageFileResponse = await fileService.SaveImage(new(request.SessionId, achievement.TrophyImageFile), existing?.TrophyImageFile);
            if (!trophyImageFileResponse.Ok)
            {
                response.AddErrors(trophyImageFileResponse.Errors);
                return;
            }

            achievement.TrophyImageFile.Id = trophyImageFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AchievementUpdate.Execute(connection, transaction, request.SessionId, achievement);
            });
        });
    }

    public async Task<Response> Remove(Request<Achievement> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var achievement = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AchievementDelete.Execute(connection, transaction, request.SessionId, achievement);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchRarityNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await RaritySelectOrderables.Execute(Connection));
    }
}