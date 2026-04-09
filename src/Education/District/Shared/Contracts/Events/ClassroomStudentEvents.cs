namespace Crudspa.Education.District.Shared.Contracts.Events;

public class ClassroomStudentPayload
{
    public Guid? Id { get; set; }
    public Guid? ClassroomId { get; set; }
}

public class ClassroomStudentAdded : ClassroomStudentPayload;

public class ClassroomStudentSaved : ClassroomStudentPayload;

public class ClassroomStudentRemoved : ClassroomStudentPayload;

public class ClassroomStudentRosterChanged : ClassroomStudentPayload;