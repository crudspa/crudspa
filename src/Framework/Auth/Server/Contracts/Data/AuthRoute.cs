namespace Crudspa.Framework.Auth.Server.Contracts.Data;

public class AuthRoute
{
    public String Key { get; set; } = null!;
    public String Name { get; set; } = null!;
    public String Provider { get; set; } = null!;
    public String? Tenant { get; set; }
    public String Audience { get; set; } = null!;
}