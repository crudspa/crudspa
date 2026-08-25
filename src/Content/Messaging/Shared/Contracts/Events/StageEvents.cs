namespace Crudspa.Content.Messaging.Shared.Contracts.Events;

public class StagePayload
{
    public Guid? Id { get; set; }
    public Guid? CampaignId { get; set; }
}

public class StageAdded : StagePayload;

public class StageSaved : StagePayload;

public class StageRemoved : StagePayload;

public class StagesReordered : StagePayload;