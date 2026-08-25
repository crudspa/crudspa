namespace Crudspa.Content.Messaging.Client.Services;

public class TokenServiceTcp(IProxyWrappers proxyWrappers) : ITokenService
{
    public async Task<Response<IList<Token>>> FetchForMembership(Request<Membership> request) =>
        await proxyWrappers.Send<IList<Token>>("TokenFetchForMembership", request);

    public async Task<Response<IList<Token>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Token>>("TokenFetchForPortal", request);
}