namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class FontPayload
{
    public Guid? Id { get; set; }
    public Guid? ContentPortalId { get; set; }
}

public class FontAdded : FontPayload;

public class FontSaved : FontPayload;

public class FontRemoved : FontPayload;