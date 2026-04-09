namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class AchievementPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class AchievementAdded : AchievementPayload;

public class AchievementSaved : AchievementPayload;

public class AchievementRemoved : AchievementPayload;