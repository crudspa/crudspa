using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

namespace Crudspa.Content.Design.Client.Plugins.RuleType;

public partial class ColorDisplay : IStyleDisplay
{
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? ContentPortalId { get; set; }

    public ColorConfig Config { get; set; } = null!;

    protected override void OnInitialized()
    {
        Config = ConfigJson.FromJson<ColorConfig>() ?? new();
    }
}