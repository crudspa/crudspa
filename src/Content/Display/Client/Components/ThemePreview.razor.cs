namespace Crudspa.Content.Display.Client.Components;

public partial class ThemePreview
{
    public enum Surfaces
    {
        None,
        Binder,
        Page,
    }

    private String? _loadedHref;
    private Boolean _ready;

    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public EventCallback Loaded { get; set; }
    [Parameter] public Guid? PortalId { get; set; }
    [Parameter] public Boolean Primary { get; set; } = true;
    [Parameter] public Surfaces Surface { get; set; }
    [Parameter] public Int32 Version { get; set; }

    public ScreenModel Model { get; } = new();

    private String Scope => PreviewCss.Scope(PortalId!.Value);
    private String Href => $"/api/content/display/styles/preview?portal={PortalId!.Value:D}&scope={Scope}&version={Version:D}";
    private String Class => PreviewCss.Class(Scope);
    private String? SurfaceStyle => Surface switch
    {
        Surfaces.Binder => "background-color: var(--binder-background-color); color: var(--binder-foreground-color);",
        Surfaces.Page => "background-color: var(--page-background-color); color: var(--page-foreground-color);",
        _ => null,
    };

    protected override Task OnParametersSetAsync()
    {
        if (!PortalId.HasValue || !Primary)
        {
            _ready = false;
            Model.Waiting = false;
            return Task.CompletedTask;
        }

        if (String.Equals(_loadedHref, Href, StringComparison.Ordinal))
            return Task.CompletedTask;

        _loadedHref = null;
        _ready = false;

        Model.WaitingOn = "Rendering...";
        Model.Waiting = true;

        return Task.CompletedTask;
    }

    private Task HandleLoaded()
    {
        _loadedHref = Href;
        _ready = true;

        Model.Waiting = false;

        return InvokeAsync(async () =>
        {
            if (Loaded.HasDelegate)
                await Loaded.InvokeAsync();

            StateHasChanged();
        });
    }

    private Task HandleError()
    {
        _loadedHref = Href;
        _ready = false;

        Model.Waiting = false;

        return InvokeAsync(StateHasChanged);
    }
}