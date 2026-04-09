namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class LessonPayload
{
    public Guid? Id { get; set; }
    public Guid? UnitId { get; set; }
}

public class LessonAdded : LessonPayload;

public class LessonSaved : LessonPayload;

public class LessonRemoved : LessonPayload;

public class LessonsReordered : LessonPayload;