using MemberSearch = Crudspa.Content.Design.Shared.Contracts.Data.MemberSearch;

namespace Crudspa.Content.Design.Client.Services;

public class MemberServiceTcp(IProxyWrappers proxyWrappers) : IMemberService
{
    public async Task<Response<IList<Member>>> SearchForMembership(Request<MemberSearch> request) =>
        await proxyWrappers.Send<IList<Member>>("MemberSearchForMembership", request);

    public async Task<Response<Member?>> Fetch(Request<Member> request) =>
        await proxyWrappers.Send<Member?>("MemberFetch", request);

    public async Task<Response<Member?>> Add(Request<Member> request) =>
        await proxyWrappers.Send<Member?>("MemberAdd", request);

    public async Task<Response> Save(Request<Member> request) =>
        await proxyWrappers.Send("MemberSave", request);

    public async Task<Response> Remove(Request<Member> request) =>
        await proxyWrappers.Send("MemberRemove", request);
}