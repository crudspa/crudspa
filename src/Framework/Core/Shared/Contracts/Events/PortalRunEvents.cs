namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class PortalRunPayload
{
    public Guid? Id { get; set; }
}

public class PortalRunChanged : PortalRunPayload;