namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class AudioElementDisplay : IElementDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;
    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;

    public AudioDisplayModel? Model { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        var model = new AudioDisplayModel(ElementModel);
        model.PropertyChanged += HandleModelChanged;
        Model = model;

        await model.DisplayModel.InitializeProgress();
    }

    public void Dispose()
    {
        Model?.PropertyChanged -= HandleModelChanged;
    }

    private async Task HandleAudioPlayed(MediaPlay mediaPlay)
    {
        if (Model is null)
            return;

        await Model.DisplayModel.AddElementCompleted();
        _ = await MediaPlayService.Add(new(mediaPlay));
    }
}

public class AudioDisplayModel(ElementDisplayModel elementModel) : Observable
{
    public ElementDisplayModel DisplayModel
    {
        get;
        set => SetProperty(ref field, value);
    } = elementModel;

    public AudioElement AudioElement
    {
        get;
        set => SetProperty(ref field, value);
    } = elementModel.RequireConfig<AudioElement>();
}