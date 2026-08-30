namespace Crudspa.Framework.Core.Server.Contracts.Data;

public class ExternalAuthRoute
{
    public String Provider { get; set; } = null!;
    public String Tenant { get; set; } = null!;
    public String Audience { get; set; } = null!;
}