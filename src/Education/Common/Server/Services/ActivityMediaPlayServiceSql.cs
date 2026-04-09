namespace Crudspa.Education.Common.Server.Services;

public class ActivityMediaPlayServiceSql(IServiceWrappers wrappers, ISqlWrappers sqlWrappers)
    : IActivityMediaPlayService
{
    public async Task<Response> Add(Request<ActivityMediaPlay> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var activityMediaPlay = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ActivityMediaPlayInsert.Execute(connection, transaction, request.SessionId, activityMediaPlay);
            });
        });
    }
}