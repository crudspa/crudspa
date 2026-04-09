namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class GameActivityPayload
{
    public Guid? Id { get; set; }
    public Guid? SectionId { get; set; }
}

public class GameActivityAdded : GameActivityPayload;

public class GameActivitySaved : GameActivityPayload;

public class GameActivityRemoved : GameActivityPayload;

public class GameActivitiesReordered : GameActivityPayload;

public class GameActivityShared : GameActivityPayload
{
    public Guid? SourceGameActivityId { get; set; }
    public Guid? TargetGameSectionId { get; set; }
}