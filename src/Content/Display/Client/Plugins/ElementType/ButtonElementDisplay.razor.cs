namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class ButtonElementDisplay : IElementDisplay
{
    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;

    public ButtonElement ButtonElement { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        ButtonElement = ElementModel.RequireConfig<ButtonElement>();
        return Task.CompletedTask;
    }

    private async Task HandleButtonClicked(ElementLink elementLink)
    {
        elementLink.ElementId = ElementModel.Element.ElementId;
        _ = await ElementProgressService.AddLink(new(elementLink));
    }
}