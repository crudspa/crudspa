namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class ContentPortalPayload
{
    public Guid? Id { get; set; }
}

public class ContentPortalSaved : ContentPortalPayload;