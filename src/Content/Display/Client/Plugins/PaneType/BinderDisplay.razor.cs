namespace Crudspa.Content.Display.Client.Plugins.PaneType;

public partial class BinderDisplay : IPaneDisplay, IHasPaneId, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Parameter] public Guid? PaneId { get; set; }

    [Inject] public IBinderRunService BinderRunService { get; set; } = null!;

    public BinderDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Id, PaneId, BinderRunService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class BinderDisplayModel(Guid? id, Guid? paneId, IBinderRunService binderRunService)
    : ScreenModel
{
    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    } = id;

    public BinderTypeFull? BinderType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        if (paneId.HasValue)
        {
            var binderPaneResponse = await WithWaiting("Fetching...", () =>
                binderRunService.FetchBinderPane(new(new() { PaneId = paneId })));

            if (binderPaneResponse.Ok)
                BinderId = binderPaneResponse.Value?.BinderId;
        }

        var response = await WithWaiting("Fetching...", () => binderRunService.FetchBinderType(new(new() { Id = BinderId })));

        if (response.Ok)
            BinderType = response.Value;
    }
}