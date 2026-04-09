namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class BinderDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IBinderRunService BinderRunService { get; set; } = null!;

    public BinderDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var config = ConfigJson.FromJson<BinderConfig>();

        if (config is not null && config.BinderId.HasSomething())
            Id = config.BinderId;

        Model = new(Id, BinderRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class BinderDisplayModel(Guid? id, IBinderRunService binderRunService)
    : ScreenModel
{
    public BinderTypeFull? BinderType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Fetching...", () => binderRunService.FetchBinderType(new(new() { Id = id })));

        if (response.Ok)
            BinderType = response.Value;
    }
}