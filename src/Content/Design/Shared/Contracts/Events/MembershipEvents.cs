namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class MembershipPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class MembershipAdded : MembershipPayload;

public class MembershipSaved : MembershipPayload;

public class MembershipRemoved : MembershipPayload;