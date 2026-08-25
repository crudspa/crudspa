using Membership = Crudspa.Content.Messaging.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface ITokenService
{
    Task<Response<IList<Token>>> FetchForMembership(Request<Membership> request);
    Task<Response<IList<Token>>> FetchForPortal(Request<Portal> request);
}