namespace Crudspa.Education.District.Shared.Contracts.Events;

public class ClassroomTeacherPayload
{
    public Guid? Id { get; set; }
    public Guid? ClassroomId { get; set; }
}

public class ClassroomTeacherAdded : ClassroomTeacherPayload;

public class ClassroomTeacherSaved : ClassroomTeacherPayload;

public class ClassroomTeacherRemoved : ClassroomTeacherPayload;

public class ClassroomTeacherRosterChanged : ClassroomTeacherPayload;