namespace Crudspa.Education.School.Shared.Contracts.Events;

public class StudentPayload
{
    public Guid? Id { get; set; }
}

public class StudentAdded : StudentPayload;

public class StudentSaved : StudentPayload;

public class StudentRemoved : StudentPayload;