namespace Crudspa.Content.Display.Server.Services;

public class SessionLicenseResolverNone : ISessionLicenseResolver
{
    public Task<IReadOnlyCollection<Guid?>> Fetch(Guid? sessionId) =>
        Task.FromResult<IReadOnlyCollection<Guid?>>([]);
}