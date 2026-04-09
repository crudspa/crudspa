namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class MemberPayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
}

public class MemberAdded : MemberPayload;

public class MemberSaved : MemberPayload;

public class MemberRemoved : MemberPayload;