
namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class PublicationPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class PublicationAdded : PublicationPayload;

public class PublicationSaved : PublicationPayload;

public class PublicationRemoved : PublicationPayload;

public class PublicationsReordered : PublicationPayload;