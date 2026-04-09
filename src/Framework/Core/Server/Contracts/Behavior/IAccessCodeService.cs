namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IAccessCodeService
{
    Task<Response> Generate(Request<User> request);
}