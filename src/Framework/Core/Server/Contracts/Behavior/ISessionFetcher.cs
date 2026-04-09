namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISessionFetcher
{
    Task<Session?> Fetch(Guid? sessionId);
    void Invalidate(Guid? sessionId);
    void InvalidateAll();
}