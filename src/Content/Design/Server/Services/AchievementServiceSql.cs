namespace Crudspa.Content.Design.Server.Services;

public class AchievementServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IAchievementService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Achievement>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Achievement>>(request, async response =>
        {
            var achievements = await AchievementSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return achievements;
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

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, achievement.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return null;
            }

            achievement.ImageFile.Id = imageFileResponse.Value.Id;

            achievement.Description = htmlSanitizer.Sanitize(achievement.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await AchievementInsert.Execute(connection, transaction, request.SessionId, achievement);

                return new Achievement
                {
                    Id = id,
                    PortalId = achievement.PortalId,
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

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, achievement.ImageFile), existing?.ImageFile);
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }

            achievement.ImageFile.Id = imageFileResponse.Value.Id;

            achievement.Description = htmlSanitizer.Sanitize(achievement.Description);

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
            var existing = await AchievementSelect.Execute(Connection, request.SessionId, achievement);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AchievementDelete.Execute(connection, transaction, request.SessionId, achievement);
            });
        });
    }
}