namespace Crudspa.Integrations.Clever.Server.Contracts.Behavior;

public interface ICleverTokenSource
{
    Task<String> Fetch(String districtId, CancellationToken cancellationToken = default);
}