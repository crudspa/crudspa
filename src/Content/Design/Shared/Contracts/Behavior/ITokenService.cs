using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ITokenService
{
    Task<Response<IList<Token>>> FetchForMembership(Request<Membership> request);
}