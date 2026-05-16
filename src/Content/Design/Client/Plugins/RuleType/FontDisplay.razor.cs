using Crudspa.Framework.Core.Shared.Contracts.Config.RuleType;

namespace Crudspa.Content.Design.Client.Plugins.RuleType;

public partial class FontDisplay : IStyleDisplay
{
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? ContentPortalId { get; set; }

    [Inject] public IFontService FontService { get; set; } = null!;

    public FontConfig Config { get; set; } = null!;
    public List<Font> Fonts { get; set; } = null!;
    public Font? Font { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Config = ConfigJson.FromJson<FontConfig>() ?? new();

        var response = await FontService.FetchForContentPortal(new(new() { Id = ContentPortalId }));

        if (response.Ok)
        {
            Fonts = response.Value.ToList();

            if (Config.Id.HasSomething())
                Font = Fonts.FirstOrDefault(x => x.Id.Equals(Config.Id));
        }
    }

    public String ToFontFaceCss()
    {
        if (Font is null)
            return String.Empty;

        return String.Join(Environment.NewLine, Font.Faces
            .Where(x => x.FileFile.Id.HasValue)
            .Select(x =>
            {
                var weight = x.WeightMax.HasValue && x.WeightMax != x.WeightMin
                    ? $"{x.WeightMin ?? 400} {x.WeightMax}"
                    : $"{x.WeightMin ?? 400}";

                return $"@font-face{{font-family:'{Font.Name!.Replace("'", "\\'")}';src:url('{x.FileFile.FetchUrl()}');font-style:{(x.Style.HasSomething() ? x.Style : "normal")};font-weight:{weight};font-display:swap;}}";
            }));
    }
}