namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class TextElementDisplay : IElementDisplay
{
    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    public TextElement TextElement { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        TextElement = ElementModel.RequireConfig<TextElement>();
        return Task.CompletedTask;
    }
}