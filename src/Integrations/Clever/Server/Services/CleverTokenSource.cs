using Crudspa.Integrations.Clever.Server.Contracts.Behavior;

namespace Crudspa.Integrations.Clever.Server.Services;

public class CleverTokenSource(CleverClient client) : ICleverTokenSource
{
    public async Task<String> Fetch(String districtId, CancellationToken cancellationToken = default) =>
        await client.FetchDistrictToken(districtId, cancellationToken);
}