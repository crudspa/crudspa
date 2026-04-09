using Crudspa.Framework.Core.Client.Components;

namespace Crudspa.Samples.Catalog.Client;

public partial class CatalogApp : IDisposable,
    IHandle<ViewImage>,
    IHandle<ViewPdf>
{
    private void HandleNavigatorChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Inject] public IPortalRunService PortalRunService { get; set; } = null!;

    public Portal? PortalRun { get; set; }
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    public ImageViewerModel? ImageViewerModel { get; set; }
    public PdfViewerModel? PdfViewerModel { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ImageViewerModel = new(ScrollService);
        PdfViewerModel = new(ScrollService);

        var response = await PortalRunService.Fetch(new(new()));

        PortalRun = response.Value;

        EventBus.Subscribe(this);

        Navigator.PropertyChanged += HandleNavigatorChanged;
    }

    public void Dispose()
    {
        Navigator.PropertyChanged -= HandleNavigatorChanged;
    }

    public async Task Handle(ViewImage payload)
    {
        if (ImageViewerModel is not null)
        {
            ImageViewerModel.ImageFile = new() { Id = payload.Id };
            await ImageViewerModel.Show();
        }
    }

    public async Task Handle(ViewPdf payload)
    {
        if (PdfViewerModel is not null)
        {
            PdfViewerModel.PdfFile = new() { Id = payload.Id };
            await PdfViewerModel.Show();
        }
    }
}