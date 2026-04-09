namespace Crudspa.Content.Display.Shared.Contracts.Events;

public class CourseProgressPayload
{
    public CourseProgress Progress { get; set; } = null!;
}

public class CourseProgressUpdated : CourseProgressPayload;

public class ContactAchievementPayload
{
    public ContactAchievement ContactAchievement { get; set; } = null!;
}

public class ContactAchievementAdded : ContactAchievementPayload;