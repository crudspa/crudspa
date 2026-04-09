namespace Crudspa.Framework.Core.Client.Components;

public partial class RootModalsCore
{
    [Parameter, EditorRequired] public Portal PortalRun { get; set; } = null!;
    [Parameter] public ImageViewerModel? ImageViewerModel { get; set; }
    [Parameter] public PdfViewerModel? PdfViewerModel { get; set; }
}