using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Design.Server.Services;

public class ThreadServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer)
    : IThreadService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Thread>>> SearchForForum(Request<ThreadSearch> request)
    {
        return await wrappers.Try<IList<Thread>>(request, async response =>
        {
            return await ThreadSelectWhereForForum.Execute(Connection, request.SessionId, request.Value);
        });
    }

    public async Task<Response<Thread?>> Fetch(Request<Thread> request)
    {
        return await wrappers.Try<Thread?>(request, async response =>
        {
            var thread = await ThreadSelect.Execute(Connection, request.SessionId, request.Value);
            return thread;
        });
    }

    public async Task<Response<Thread?>> Add(Request<Thread> request)
    {
        return await wrappers.Validate<Thread?, Thread>(request, async response =>
        {
            var thread = request.Value;

            thread.Comment.Body = htmlSanitizer.Sanitize(thread.Comment.Body);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ThreadInsert.Execute(connection, transaction, request.SessionId, thread);

                return new Thread
                {
                    Id = id,
                    ForumId = thread.ForumId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Thread> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var thread = request.Value;

            thread.Comment.Body = htmlSanitizer.Sanitize(thread.Comment.Body);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ThreadUpdate.Execute(connection, transaction, request.SessionId, thread);
            });
        });
    }

    public async Task<Response> Remove(Request<Thread> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var thread = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ThreadDelete.Execute(connection, transaction, request.SessionId, thread);
            });
        });
    }
}