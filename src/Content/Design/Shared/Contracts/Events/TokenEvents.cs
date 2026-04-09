namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class TokenPayload
{
    public Guid? Id { get; set; }
    public Guid? MembershipId { get; set; }
}

public class TokensReordered : TokenPayload;