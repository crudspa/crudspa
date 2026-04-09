namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class EmailPayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
}

public class EmailAdded : EmailPayload;

public class EmailSaved : EmailPayload;

public class EmailRemoved : EmailPayload;