namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface ISessionStateService
{
    Task<Response<Session?>> Fetch(Request<Session> request);
}