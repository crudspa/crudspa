namespace Crudspa.Content.Messaging.Server.Services;

public class TokenServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : ITokenService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Token>>> FetchForMembership(Request<Membership> request)
    {
        return await wrappers.Try<IList<Token>>(request, async response =>
        {
            var tokens = await TokenSelectForMembership.Execute(Connection, request.SessionId, request.Value.Id);

            return tokens;
        });
    }

    public async Task<Response<IList<Token>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Token>>(request, async response =>
        {
            var tokens = await TokenSelectForPortal.Execute(Connection, request.SessionId, request.Value.Id);

            return tokens;
        });
    }
}