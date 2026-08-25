namespace Crudspa.Content.Messaging.Shared.Contracts.Events;

public class PopulationPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class PopulationAdded : PopulationPayload;

public class PopulationSaved : PopulationPayload;

public class PopulationRemoved : PopulationPayload;