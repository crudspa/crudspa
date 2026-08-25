namespace Crudspa.Content.Display.Server.Services;

public class ForumMediaServiceSql(
    IServerConfigService configService,
    ISessionLicenseResolver sessionLicenseResolver)
    : IForumMediaService
{
    private String Connection => configService.Fetch().Database;

    public async Task<ForumMedia?> Fetch(Guid? sessionId, Guid? commentMediaId)
    {
        var licenseIds = await sessionLicenseResolver.Fetch(sessionId);
        return await ForumRunMediaSelect.Execute(Connection, sessionId, commentMediaId, licenseIds);
    }

    public async Task<ForumUploadStageResults> Stage(Guid? sessionId, Guid? forumId, ForumUploadStage upload)
    {
        var licenseIds = await sessionLicenseResolver.Fetch(sessionId);
        return await ForumRunUploadStage.Insert(Connection, sessionId, forumId, upload, licenseIds);
    }

    public async Task<ForumUploadStage?> Consume(Guid? sessionId, Guid? forumId, CommentMedia.Types type, Guid? blobId)
    {
        var licenseIds = await sessionLicenseResolver.Fetch(sessionId);
        return await ForumRunUploadStage.Consume(Connection, sessionId, forumId, type, blobId, licenseIds);
    }

    public async Task Discard(Guid? sessionId, Guid? blobId) =>
        await ForumRunUploadStage.Discard(Connection, sessionId, blobId);

    public async Task<IList<Guid>> FetchExpired() =>
        await ForumRunUploadStage.FetchExpired(Connection);

    public async Task DiscardExpired(Guid blobId) =>
        await ForumRunUploadStage.DiscardExpired(Connection, blobId);
}