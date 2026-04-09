using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

namespace Crudspa.Content.Design.Client.Plugins.RuleType;

public partial class MarginDesign : IStyleDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? ContentPortalId { get; set; }

    public MarginConfig Config { get; set; } = null!;

    protected override void OnInitialized()
    {
        Config = ConfigJson.FromJson<MarginConfig>() ?? new();
    }

    public String? GetConfigJson() => Config.ToJson();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            Config.Top.ValidateWidthProperty(nameof(Config.Top), errors);
            Config.Right.ValidateWidthProperty(nameof(Config.Right), errors);
            Config.Bottom.ValidateWidthProperty(nameof(Config.Bottom), errors);
            Config.Left.ValidateWidthProperty(nameof(Config.Left), errors);
        });
    }
}