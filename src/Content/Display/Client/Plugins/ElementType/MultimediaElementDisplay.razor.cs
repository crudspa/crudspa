namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class MultimediaElementDisplay : IElementDisplay
{
    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;

    public MultimediaElement MultimediaElement { get; set; } = null!;
    public IReadOnlyList<MultimediaItem> MultimediaItems { get; set; } = [];

    protected override Task OnInitializedAsync()
    {
        MultimediaElement = ElementModel.RequireConfig<MultimediaElement>();
        MultimediaItems = MultimediaElement.MultimediaItems.OrderBy(x => x.Ordinal).ToList();
        return Task.CompletedTask;
    }

    private async Task HandleAudioPlayed(MediaPlay mediaPlay)
    {
        _ = await MediaPlayService.Add(new(mediaPlay));
    }

    private async Task HandleVideoPlayed(MediaPlay mediaPlay)
    {
        _ = await MediaPlayService.Add(new(mediaPlay));
    }

    private async Task HandleButtonClicked(ElementLink elementLink)
    {
        elementLink.ElementId = ElementModel.Element.ElementId;
        _ = await ElementProgressService.AddLink(new(elementLink));
    }
}