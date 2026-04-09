namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ReadPartPayload
{
    public Guid? Id { get; set; }
    public Guid? AssessmentId { get; set; }
}

public class ReadPartAdded : ReadPartPayload;

public class ReadPartSaved : ReadPartPayload;

public class ReadPartRemoved : ReadPartPayload;

public class ReadPartsReordered : ReadPartPayload;