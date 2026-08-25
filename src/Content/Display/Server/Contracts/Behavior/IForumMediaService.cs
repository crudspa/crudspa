namespace Crudspa.Content.Display.Server.Contracts.Behavior;

public enum ForumUploadStageResults
{
    Denied,
    Staged,
    PendingQuotaExceeded,
    DailyQuotaExceeded,
}

public interface IForumMediaService
{
    Task<ForumMedia?> Fetch(Guid? sessionId, Guid? commentMediaId);
    Task<ForumUploadStageResults> Stage(Guid? sessionId, Guid? forumId, ForumUploadStage upload);
    Task<ForumUploadStage?> Consume(Guid? sessionId, Guid? forumId, CommentMedia.Types type, Guid? blobId);
    Task Discard(Guid? sessionId, Guid? blobId);
    Task<IList<Guid>> FetchExpired();
    Task DiscardExpired(Guid blobId);
}