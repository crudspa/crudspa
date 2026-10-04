
namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class PublicationGroupPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class PublicationGroupAdded : PublicationGroupPayload;

public class PublicationGroupSaved : PublicationGroupPayload;

public class PublicationGroupRemoved : PublicationGroupPayload;

public class PublicationGroupsReordered : PublicationGroupPayload;