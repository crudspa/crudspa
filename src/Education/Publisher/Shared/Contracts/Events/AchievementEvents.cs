namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class AchievementPayload
{
    public Guid? Id { get; set; }
}

public class AchievementAdded : AchievementPayload;

public class AchievementSaved : AchievementPayload;

public class AchievementRemoved : AchievementPayload;