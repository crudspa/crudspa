namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IStylesRunService
{
    Task<PortalStyles?> FetchForPortal(Guid portalId);
    Task<Int32?> FetchStyleRevision(Guid portalId);
}