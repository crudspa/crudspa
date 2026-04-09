namespace Crudspa.Framework.Core.Client.Components;

public partial class ImageViewer
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public ImageViewerModel Model { get; set; } = null!;

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

public class ImageViewerModel(IScrollService scrollService)
    : ModalModel(scrollService)
{
    public ImageFile? ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override async Task Show()
    {
        if (ImageFile?.Id is null)
            return;

        await base.Show();
    }

    public override async Task Hide()
    {
        await base.Hide();
        ImageFile = null;
    }
}