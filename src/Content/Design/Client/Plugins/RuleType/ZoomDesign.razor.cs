using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

namespace Crudspa.Content.Design.Client.Plugins.RuleType;

public partial class ZoomDesign : IStyleDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? ContentPortalId { get; set; }

    public ZoomConfig Config { get; set; } = null!;

    protected override void OnInitialized()
    {
        Config = ConfigJson.FromJson<ZoomConfig>() ?? new();
    }

    public String? GetConfigJson() => Config.ToJson();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}