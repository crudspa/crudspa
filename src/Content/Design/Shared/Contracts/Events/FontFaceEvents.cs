namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class FontFacePayload
{
    public Guid? Id { get; set; }
    public Guid? FontId { get; set; }
    public Guid? ContentPortalId { get; set; }
}

public class FontFaceAdded : FontFacePayload;

public class FontFaceSaved : FontFacePayload;

public class FontFaceRemoved : FontFacePayload;