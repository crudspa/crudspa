namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class VocabPartPayload
{
    public Guid? Id { get; set; }
    public Guid? AssessmentId { get; set; }
}

public class VocabPartAdded : VocabPartPayload;

public class VocabPartSaved : VocabPartPayload;

public class VocabPartRemoved : VocabPartPayload;

public class VocabPartsReordered : VocabPartPayload;