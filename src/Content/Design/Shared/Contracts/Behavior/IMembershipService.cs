using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IMembershipService
{
    Task<Response<IList<Membership>>> FetchForPortal(Request<Portal> request);
    Task<Response<Membership?>> Fetch(Request<Membership> request);
    Task<Response<Membership?>> Add(Request<Membership> request);
    Task<Response> Save(Request<Membership> request);
    Task<Response> Remove(Request<Membership> request);
    Task<Response> CreateMembership(Request<Membership> request);
    Task<Response> CreateTokens(Request<Membership> request);
}