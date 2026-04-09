namespace Crudspa.Content.Jobs.Server.Services;

public class ContentActionServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService,
    ISqlWrappers sqlWrappers)
    : IContentActionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Email>>> FetchEmailsForSending(Request request)
    {
        return await wrappers.Try<IList<Email>>(request, async response =>
            await EmailSelectForSending.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Member>>> FetchMembers(Request<Membership> request)
    {
        return await wrappers.Try<IList<Member>>(request, async response =>
            await MemberSelectForSending.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response> SaveLog(Request<EmailLog> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await EmailLogInsert.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response> UpdateStatus(Request<Email> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var email = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await EmailUpdateStatus.Execute(connection, transaction, request.SessionId, email.Id!.Value, email.Status);
            });
        });
    }
}