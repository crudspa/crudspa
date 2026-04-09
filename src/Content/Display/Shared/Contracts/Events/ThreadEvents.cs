namespace Crudspa.Content.Display.Shared.Contracts.Events;

public class ThreadPayload
{
    public Guid? Id { get; set; }
    public Guid? ForumId { get; set; }
}

public class ThreadAdded : ThreadPayload;

public class ThreadSaved : ThreadPayload;

public class ThreadRemoved : ThreadPayload;