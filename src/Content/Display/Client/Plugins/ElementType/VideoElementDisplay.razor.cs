namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class VideoElementDisplay : IElementDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;
    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;

    public VideoDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var videoElement = ElementModel.RequireConfig<VideoElement>();

        Model = new(ElementModel, videoElement);
        Model.PropertyChanged += HandleModelChanged;

        await Model.ElementModel.InitializeProgress();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
    }

    private async Task HandleVideoPlayed(MediaPlay mediaPlay)
    {
        await Model.ElementModel.AddElementCompleted();
        _ = await MediaPlayService.Add(new(mediaPlay));
    }
}

public class VideoDisplayModel(ElementDisplayModel elementModel, VideoElement videoElement) : Observable
{
    public ElementDisplayModel ElementModel => elementModel;

    public VideoElement VideoElement
    {
        get;
        set => SetProperty(ref field, value);
    } = videoElement;
}