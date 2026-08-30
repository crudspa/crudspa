namespace Crudspa.Framework.Auth.Server.Contracts.Data;

public class AuthHandoffRedemption
{
    public Guid? UserId { get; set; }
    public Guid? ExternalIdentityId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? AuthPolicyId { get; set; }
    public DateTimeOffset? AbsoluteExpires { get; set; }
    public String? ReturnPath { get; set; }
    public Boolean Persist { get; set; }
}