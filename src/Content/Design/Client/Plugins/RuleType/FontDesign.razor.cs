using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

namespace Crudspa.Content.Design.Client.Plugins.RuleType;

public partial class FontDesign : IStyleDesign
{
    [CascadingParameter(Name = nameof(ReadOnly))] public Boolean ReadOnly { get; set; }

    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? ContentPortalId { get; set; }

    [Inject] public IFontService FontService { get; set; } = null!;

    public FontConfig Config { get; set; } = null!;
    public List<Font> Fonts { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<FontConfig>() ?? new();

        var response = await FontService.FetchForContentPortal(new(new() { Id = ContentPortalId }));

        if (response.Ok)
            Fonts = response.Value.ToList();
    }

    public String? GetConfigJson() => Config.ToJson();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            Config.Size.ValidateWidthProperty(nameof(Config.Size), errors);
            Config.Weight.ValidateWeightProperty(nameof(Config.Weight), errors);
        });
    }
}