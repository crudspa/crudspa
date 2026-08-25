namespace Crudspa.Content.Messaging.Server.Services;

public class MessageServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IMessageService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Message>>> SearchForMembership(Request<MessageSearch> request)
    {
        return await wrappers.Try<IList<Message>>(request, async response =>
        {
            var messages = await MessageSelectWhereForMembership.Execute(Connection, request.SessionId, request.Value);

            return messages;
        });
    }

    public async Task<Response<IList<Message>>> FetchForActivation(Request<Activation> request)
    {
        return await wrappers.Try<IList<Message>>(request, async response =>
        {
            var messages = await MessageSelectForActivation.Execute(Connection, request.SessionId, request.Value.Id);

            return messages;
        });
    }

    public async Task<Response<IList<Message>>> FetchForStage(Request<Stage> request) => await wrappers.Try<IList<Message>>(request, async response => await MessageSelectForStage.Execute(Connection,request.SessionId,request.Value.Id));
    public async Task<Response<Message?>> Fetch(Request<Message> request) => await wrappers.Try<Message?>(request, async response => await MessageSelect.Execute(Connection,request.SessionId,request.Value.Id));
    public async Task<Response<Message?>> Add(Request<Message> request) => await wrappers.Validate<Message?,Message>(request,async response=>await sqlWrappers.WithConnection(async(connection,transaction)=>new Message{Id=await MessageInsert.Execute(connection,transaction,request.SessionId,request.Value),StageId=request.Value.StageId}));
    public async Task<Response> Save(Request<Message> request) => await wrappers.Validate(request,async response=>await sqlWrappers.WithConnection(async(connection,transaction)=>await MessageUpdate.Execute(connection,transaction,request.SessionId,request.Value)));
    public async Task<Response> Remove(Request<Message> request) => await wrappers.Try(request,async response=>await sqlWrappers.WithConnection(async(connection,transaction)=>await MessageDelete.Execute(connection,transaction,request.SessionId,request.Value.Id)));
}