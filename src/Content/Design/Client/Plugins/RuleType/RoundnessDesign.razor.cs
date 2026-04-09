using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

namespace Crudspa.Content.Design.Client.Plugins.RuleType;

public partial class RoundnessDesign : IStyleDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? ContentPortalId { get; set; }

    public RoundnessConfig Config { get; set; } = null!;

    protected override void OnInitialized()
    {
        Config = ConfigJson.FromJson<RoundnessConfig>() ?? new();
    }

    public String? GetConfigJson() => Config.ToJson();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            Config.Radius.ValidateWidthProperty(nameof(Config.Radius), errors);
        });
    }
}