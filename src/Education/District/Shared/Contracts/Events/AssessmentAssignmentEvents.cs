namespace Crudspa.Education.District.Shared.Contracts.Events;

public class AssessmentAssignmentPayload
{
    public Guid? Id { get; set; }
}

public class AssessmentAssignmentAdded : AssessmentAssignmentPayload;

public class AssessmentAssignmentSaved : AssessmentAssignmentPayload;

public class AssessmentAssignmentRemoved : AssessmentAssignmentPayload;