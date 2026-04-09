using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Client.Services;

public class TokenServiceTcp(IProxyWrappers proxyWrappers) : ITokenService
{
    public async Task<Response<IList<Token>>> FetchForMembership(Request<Membership> request) =>
        await proxyWrappers.Send<IList<Token>>("TokenFetchForMembership", request);
}