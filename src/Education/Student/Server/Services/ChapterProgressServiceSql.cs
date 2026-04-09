namespace Crudspa.Education.Student.Server.Services;

public class ChapterProgressServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IChapterProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ChapterProgress>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<ChapterProgress>>(request, async response =>
            await ChapterProgressSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<ChapterProgress> Fetch(Request<Chapter> request)
    {
        return await ChapterProgressSelect.Execute(Connection, request.SessionId, request.Value.Id);
    }

    public async Task<Response> AddCompleted(Request<ChapterCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var chapterCompleted = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ChapterCompletedInsert.Execute(connection, transaction, request.SessionId, chapterCompleted);

                var allAreComplete = await ChapterAllAreComplete.Execute(Connection, request.SessionId, chapterCompleted.ChapterId);

                if (allAreComplete)
                    await ContentCompletedInsertByChapter.Execute(connection, transaction, request.SessionId, chapterCompleted.ChapterId);
            });
        });
    }
}