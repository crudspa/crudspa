namespace Crudspa.Content.Messaging.Server.Services;

public class SmsEventServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : ISmsEventService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SmsEvent>>> Search(Request<SmsEventSearch> request)
    {
        return await wrappers.Try<IList<SmsEvent>>(request, async response =>
        {
            var smsEvents = await SmsEventSelectWhere.Execute(Connection, request.SessionId, request.Value);

            return smsEvents;
        });
    }

    public async Task<Response<IList<SmsEvent>>> SearchForSmsMessage(Request<SmsEventSearch> request)
    {
        return await wrappers.Try<IList<SmsEvent>>(request, async response =>
        {
            var smsEvents = await SmsEventSelectWhereForSmsMessage.Execute(Connection, request.SessionId, request.Value);

            return smsEvents;
        });
    }
}