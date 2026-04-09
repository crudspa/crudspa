namespace Crudspa.Content.Design.Client.Components;

public partial class PageSections
{
    private Guid? _themePortalId;
    private Int32 _themeVersion;

    [Parameter, EditorRequired] public PageSectionsModelBase Model { get; set; } = null!;
    [Parameter, EditorRequired] public String? Path { get; set; }
    [Parameter] public String? SectionPathPrefix { get; set; }
    [Parameter] public Guid? PreviewPortalId { get; set; }
    [Parameter] public Int32 PreviewVersion { get; set; }

    public ScreenModel ThemeModel { get; } = new();
    public Boolean ThemeReady { get; set; } = true;
    public String? EffectiveSectionPathPrefix => SectionPathPrefix ?? Path;

    protected override void OnParametersSet()
    {
        if (!PreviewPortalId.HasValue)
        {
            ThemeReady = true;
            ThemeModel.Waiting = false;
            return;
        }

        if (!PreviewPortalId.Equals(_themePortalId) || PreviewVersion != _themeVersion)
        {
            _themePortalId = PreviewPortalId;
            _themeVersion = PreviewVersion;

            ThemeReady = false;
            ThemeModel.WaitingOn = "Rendering...";
            ThemeModel.Waiting = true;
        }
    }

    private Task HandleThemeLoaded()
    {
        ThemeReady = true;
        ThemeModel.Waiting = false;
        return InvokeAsync(StateHasChanged);
    }
}