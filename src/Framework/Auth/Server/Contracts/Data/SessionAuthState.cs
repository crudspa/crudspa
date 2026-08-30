namespace Crudspa.Framework.Auth.Server.Contracts.Data;

public class SessionAuthState
{
    public Guid? AuthPolicyId { get; set; }
    public DateTimeOffset? LastActivity { get; set; }
    public Int32 IdleTimeoutMinutes { get; set; }
    public DateTimeOffset? IdleExpires { get; set; }
    public DateTimeOffset? AbsoluteExpires { get; set; }
}