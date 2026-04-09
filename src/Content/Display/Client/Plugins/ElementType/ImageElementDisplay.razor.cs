using Microsoft.AspNetCore.Components.Web;

namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class ImageElementDisplay : IElementDisplay
{
    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;

    public ImageElement ImageElement { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        ImageElement = ElementModel.RequireConfig<ImageElement>();
        return Task.CompletedTask;
    }

    private async Task HandleClick(MouseEventArgs args)
    {
        _ = await ElementProgressService.AddLink(new(new()
        {
            ElementId = ImageElement.ElementId,
            Url = ImageElement.HyperlinkUrl,
        }));
    }
}