namespace Crudspa.Integrations.Clever.Server.Contracts.Data;

public class CleverConfig(IConfiguration configuration)
{
    private const String Prefix = "Crudspa.Integrations.Clever.Server";

    public Boolean Enabled => Boolean.TryParse(configuration[$"{Prefix}.Enabled"], out var enabled) && enabled;
    public String? ClientId => configuration[$"{Prefix}.ClientId"];
    public String? ClientSecret => configuration[$"{Prefix}.ClientSecret"];
    public Boolean Configured => !String.IsNullOrWhiteSpace(ClientId) && !String.IsNullOrWhiteSpace(ClientSecret);
}