using Membership = Crudspa.Content.Messaging.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IMembershipService
{
    Task<Response<IList<Membership>>> FetchForPortal(Request<Portal> request);
    Task<Response<Membership?>> Fetch(Request<Membership> request);
    Task<Response<Membership?>> Add(Request<Membership> request);
    Task<Response> Save(Request<Membership> request);
    Task<Response> Remove(Request<Membership> request);
}