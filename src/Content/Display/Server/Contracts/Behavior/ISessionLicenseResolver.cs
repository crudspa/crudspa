namespace Crudspa.Content.Display.Server.Contracts.Behavior;

public interface ISessionLicenseResolver
{
    Task<IReadOnlyCollection<Guid?>> Fetch(Guid? sessionId);
}