namespace Crudspa.Education.School.Shared.Contracts.Events;

public class PostPayload
{
    public Guid? Id { get; set; }
    public Guid? ForumId { get; set; }
    public Guid? ParentId { get; set; }
}

public class PostAdded : PostPayload;

public class PostSaved : PostPayload;

public class PostRemoved : PostPayload;