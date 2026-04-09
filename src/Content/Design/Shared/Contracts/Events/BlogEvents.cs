namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class BlogPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class BlogAdded : BlogPayload;

public class BlogSaved : BlogPayload;

public class BlogRemoved : BlogPayload;