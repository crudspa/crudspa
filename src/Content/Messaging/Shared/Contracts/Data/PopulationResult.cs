namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class PopulationResult
{
    public IList<PopulationMember> Members { get; set; } = [];
    public IList<PopulationToken> Tokens { get; set; } = [];
    public IList<PopulationTokenValue> TokenValues { get; set; } = [];
}

public class PopulationMember
{
    public Guid ContactId { get; set; }
}

public class PopulationToken
{
    public String Key { get; set; } = String.Empty;
    public String? Description { get; set; }
    public Int32 Ordinal { get; set; }
}

public class PopulationTokenValue
{
    public Guid ContactId { get; set; }
    public String Key { get; set; } = String.Empty;
    public String? Value { get; set; }
}