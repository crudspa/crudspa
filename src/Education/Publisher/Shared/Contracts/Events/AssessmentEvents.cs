namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class AssessmentPayload
{
    public Guid? Id { get; set; }
}

public class AssessmentAdded : AssessmentPayload;

public class AssessmentSaved : AssessmentPayload;

public class AssessmentRemoved : AssessmentPayload;