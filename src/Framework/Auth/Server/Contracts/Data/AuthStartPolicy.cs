namespace Crudspa.Framework.Auth.Server.Contracts.Data;

public class AuthStartPolicy
{
    public String? Provider { get; set; }
    public String? Tenant { get; set; }
    public Boolean AutoRedirect { get; set; }
    public Boolean Fallback { get; set; }
}