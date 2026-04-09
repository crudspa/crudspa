namespace Crudspa.Framework.Core.Server.Services;

public class LinkClickServiceSql(IServiceWrappers wrappers, ISqlWrappers sqlWrappers) : ILinkClickService
{
    public async Task<Response> Add(Request<LinkClick> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var linkClick = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await LinkFollowedInsert.Execute(connection, transaction, request.SessionId, linkClick.Url);
            });
        });
    }
}