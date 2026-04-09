namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ObjectivePayload
{
    public Guid? Id { get; set; }
    public Guid? LessonId { get; set; }
}

public class ObjectiveAdded : ObjectivePayload;

public class ObjectiveSaved : ObjectivePayload;

public class ObjectiveRemoved : ObjectivePayload;

public class ObjectivesReordered : ObjectivePayload;