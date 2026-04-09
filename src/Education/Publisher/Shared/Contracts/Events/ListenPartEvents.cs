namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ListenPartPayload
{
    public Guid? Id { get; set; }
    public Guid? AssessmentId { get; set; }
}

public class ListenPartAdded : ListenPartPayload;

public class ListenPartSaved : ListenPartPayload;

public class ListenPartRemoved : ListenPartPayload;

public class ListenPartsReordered : ListenPartPayload;