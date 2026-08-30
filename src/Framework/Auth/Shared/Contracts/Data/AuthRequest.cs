namespace Crudspa.Framework.Auth.Shared.Contracts.Data;

public class AuthRequest
{
    public String? Audience { get; set; }
    public String? Tenant { get; set; }
    public String? ReturnPath { get; set; }
}