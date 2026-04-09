namespace Crudspa.Framework.Core.Client.Components;

public partial class PdfViewer
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public PdfViewerModel Model { get; set; } = null!;

    public String SourceUrl => Model.PdfFile.FetchUrl();

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class PdfViewerModel(IScrollService scrollService)
    : ModalModel(scrollService)
{
    public PdfFile? PdfFile
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override async Task Show()
    {
        if (PdfFile?.Id is null)
            return;

        await base.Show();
    }

    public override async Task Hide()
    {
        await base.Hide();
        PdfFile = null;
    }
}