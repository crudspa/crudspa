namespace Crudspa.Education.School.Shared.Contracts.Events;

public class SchoolContactPayload
{
    public Guid? Id { get; set; }
    public Guid? SchoolId { get; set; }
}

public class SchoolContactAdded : SchoolContactPayload;

public class SchoolContactSaved : SchoolContactPayload;

public class SchoolContactRemoved : SchoolContactPayload;

public class SchoolContactRelationsSaved : SchoolContactPayload;