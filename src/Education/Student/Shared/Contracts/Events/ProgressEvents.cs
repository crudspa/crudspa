namespace Crudspa.Education.Student.Shared.Contracts.Events;

public class BookProgressPayload
{
    public BookProgress Progress { get; set; } = null!;
}

public class BookProgressUpdated : BookProgressPayload;

public class ChapterProgressPayload
{
    public ChapterProgress Progress { get; set; } = null!;
}

public class ChapterProgressUpdated : ChapterProgressPayload;

public class GameProgressPayload
{
    public GameProgress Progress { get; set; } = null!;
}

public class GameProgressUpdated : GameProgressPayload;

public class ModuleProgressPayload
{
    public ModuleProgress Progress { get; set; } = null!;
}

public class ModuleProgressUpdated : ModuleProgressPayload;

public class ObjectiveProgressPayload
{
    public ObjectiveProgress Progress { get; set; } = null!;
}

public class ObjectiveProgressUpdated : ObjectiveProgressPayload;

public class TrifoldProgressPayload
{
    public TrifoldProgress Progress { get; set; } = null!;
}

public class TrifoldProgressUpdated : TrifoldProgressPayload;

public class StudentAchievementPayload
{
    public StudentAchievement StudentAchievement { get; set; } = null!;
}

public class StudentAchievementAdded : StudentAchievementPayload;