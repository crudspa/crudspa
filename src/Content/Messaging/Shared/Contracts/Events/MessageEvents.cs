namespace Crudspa.Content.Messaging.Shared.Contracts.Events;

public class MessagePayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
    public Guid? ActivationId { get; set; }
    public Guid? StageId { get; set; }
}

public class MessageAdded : MessagePayload;

public class MessageSaved : MessagePayload;

public class MessageRemoved : MessagePayload;