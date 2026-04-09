namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class PdfElementDisplay : IElementDisplay
{
    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IEventBus EventBus { get; set; } = null!;

    public PdfElement PdfElement { get; set; } = null!;

    public String DownloadUrl => PdfElement.FileFile.FetchUrl(true);

    protected override Task OnInitializedAsync()
    {
        PdfElement = ElementModel.RequireConfig<PdfElement>();
        return Task.CompletedTask;
    }
}