namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class StylePayload
{
    public Guid? Id { get; set; }
    public Guid? ContentPortalId { get; set; }
}

public class StyleSaved : StylePayload;