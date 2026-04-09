namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ForumPayload
{
    public Guid? Id { get; set; }
}

public class ForumAdded : ForumPayload;

public class ForumSaved : ForumPayload;

public class ForumRemoved : ForumPayload;