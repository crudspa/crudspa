namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class CoursePayload
{
    public Guid? Id { get; set; }
    public Guid? TrackId { get; set; }
}

public class CourseAdded : CoursePayload;

public class CourseSaved : CoursePayload;

public class CourseRemoved : CoursePayload;

public class CoursesReordered : CoursePayload;