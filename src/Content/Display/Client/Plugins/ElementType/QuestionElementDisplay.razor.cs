namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class QuestionElementDisplay : IElementDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;

    public QuestionDisplayModel? Model { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        var model = new QuestionDisplayModel(EventBus, ElementProgressService, ElementModel);
        model.PropertyChanged += HandleModelChanged;
        Model = model;

        await model.Initialize();
    }

    public void Dispose()
    {
        if (Model is not null)
        {
            Model.PropertyChanged -= HandleModelChanged;
            Model.Dispose();
        }
    }
}