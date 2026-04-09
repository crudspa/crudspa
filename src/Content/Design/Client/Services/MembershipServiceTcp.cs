using Crudspa.Framework.Core.Client.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Client.Services;

public class MembershipServiceTcp(IProxyWrappers proxyWrappers) : IMembershipService
{
    public async Task<Response<IList<Membership>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Membership>>("MembershipFetchForPortal", request);

    public async Task<Response<Membership?>> Fetch(Request<Membership> request) =>
        await proxyWrappers.Send<Membership?>("MembershipFetch", request);

    public async Task<Response<Membership?>> Add(Request<Membership> request) =>
        await proxyWrappers.Send<Membership?>("MembershipAdd", request);

    public async Task<Response> Save(Request<Membership> request) =>
        await proxyWrappers.Send("MembershipSave", request);

    public async Task<Response> Remove(Request<Membership> request) =>
        await proxyWrappers.Send("MembershipRemove", request);

    public async Task<Response> CreateMembership(Request<Membership> request) =>
        await proxyWrappers.Send("MembershipCreate", request);

    public async Task<Response> CreateTokens(Request<Membership> request) =>
        await proxyWrappers.Send("TokensCreate", request);
}