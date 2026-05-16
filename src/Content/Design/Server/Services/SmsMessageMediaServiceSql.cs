namespace Crudspa.Content.Design.Server.Services;

public class SmsMessageMediaServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ISmsMessageMediaService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SmsMessageMedia>>> FetchForSmsMessage(Request<SmsMessage> request)
    {
        return await wrappers.Try<IList<SmsMessageMedia>>(request, async response =>
        {
            var smsMessageMedias = await SmsMessageMediaSelectForSmsMessage.Execute(Connection, request.SessionId, request.Value.Id);

            return smsMessageMedias;
        });
    }

    public async Task<Response> SaveOrder(Request<IList<SmsMessageMedia>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var smsMessageMedias = request.Value;

            smsMessageMedias.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SmsMessageMediaUpdateOrdinals.Execute(connection, transaction, request.SessionId, smsMessageMedias);
            });
        });
    }
}