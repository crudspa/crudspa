namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class PortalPayload
{
    public Guid? Id { get; set; }
}

public class PortalAdded : PortalPayload;

public class PortalSaved : PortalPayload;

public class PortalRemoved : PortalPayload;