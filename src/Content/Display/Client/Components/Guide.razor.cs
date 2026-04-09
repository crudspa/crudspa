namespace Crudspa.Content.Display.Client.Components;

public partial class Guide : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public GuideModel Model { get; set; } = null!;
    [Parameter] public Boolean ShowGuideFields { get; set; } = true;
    [Parameter] public Boolean ShowNotebookButton { get; set; }
    [Parameter] public Boolean AutoPlay { get; set; } = true;

    [Inject] public IEventBus EventBus { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
    }
}

public class GuideModel : Observable
{
    public ImageFile? Image
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile? Audio
    {
        get;
        set => SetProperty(ref field, value);
    }
}