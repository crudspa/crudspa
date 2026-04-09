namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class PostPayload
{
    public Guid? Id { get; set; }
    public Guid? BlogId { get; set; }
}

public class PostAdded : PostPayload;

public class PostSaved : PostPayload;

public class PostRemoved : PostPayload;