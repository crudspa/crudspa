namespace Crudspa.Education.District.Shared.Contracts.Events;

public class ClassroomPayload
{
    public Guid? Id { get; set; }
}

public class ClassroomAdded : ClassroomPayload;

public class ClassroomSaved : ClassroomPayload;

public class ClassroomRemoved : ClassroomPayload;