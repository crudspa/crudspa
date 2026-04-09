using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

namespace Crudspa.Content.Design.Client.Plugins.RuleType;

public partial class ColorPairDesign : IStyleDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? ContentPortalId { get; set; }

    public ColorPairConfig Config { get; set; } = null!;

    protected override void OnInitialized()
    {
        Config = ConfigJson.FromJson<ColorPairConfig>() ?? new();
    }

    public String? GetConfigJson() => Config.ToJson();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            Config.Background.ValidateColorProperty(nameof(Config.Background), errors);
            Config.Foreground.ValidateColorProperty(nameof(Config.Foreground), errors);
        });
    }
}