namespace Crudspa.Framework.Core.Server.Services;

public class MediaPlayServiceSql(IServiceWrappers wrappers, ISqlWrappers sqlWrappers) : IMediaPlayService
{
    public async Task<Response> Add(Request<MediaPlay> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var mediaPlay = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MediaPlayInsert.Execute(connection, transaction, request.SessionId, mediaPlay);
            });
        });
    }
}