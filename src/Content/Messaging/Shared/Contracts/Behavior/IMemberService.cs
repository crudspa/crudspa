using MemberSearch = Crudspa.Content.Messaging.Shared.Contracts.Data.MemberSearch;

namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IMemberService
{
    Task<Response<IList<Member>>> SearchForMembership(Request<MemberSearch> request);
    Task<Response<Member?>> Fetch(Request<Member> request);
    Task<Response<Member?>> Add(Request<Member> request);
    Task<Response> Save(Request<Member> request);
    Task<Response> Remove(Request<Member> request);
}