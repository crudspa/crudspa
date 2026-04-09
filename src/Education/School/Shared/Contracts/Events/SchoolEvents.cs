namespace Crudspa.Education.School.Shared.Contracts.Events;

public class SchoolPayload
{
    public Guid? Id { get; set; }
}

public class SchoolAdded : SchoolPayload;

public class SchoolSaved : SchoolPayload;

public class SchoolRemoved : SchoolPayload;