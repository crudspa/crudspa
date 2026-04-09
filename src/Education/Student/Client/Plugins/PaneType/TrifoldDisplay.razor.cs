namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class TrifoldDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public ITrifoldProgressService TrifoldProgressService { get; set; } = null!;

    public TrifoldDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, StudentAppService, TrifoldProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.Dispose();
        Model.PropertyChanged -= HandleModelChanged;
    }
}

public class TrifoldDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IStudentAppService _studentAppService;
    private readonly ITrifoldProgressService _trifoldProgressService;
    private Trifold? _trifold;

    public TrifoldDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IStudentAppService studentAppService,
        ITrifoldProgressService trifoldProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _studentAppService = studentAppService;
        _trifoldProgressService = trifoldProgressService;

        eventBus.Subscribe(this);
    }

    public Trifold? Trifold
    {
        get => _trifold;
        set => SetProperty(ref _trifold, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchTrifold(new(new() { Id = _id })));

        if (response.Ok)
            await SetTrifold(response.Value);
    }

    public async Task HandleBinderCompleted()
    {
        await _trifoldProgressService.AddCompleted(new(new()
        {
            TrifoldId = _trifold!.Id,
            DeviceTimestamp = DateTimeOffset.Now,
        }));

        _navigator.Close(_path);
    }

    private Task SetTrifold(Trifold trifold)
    {
        Trifold = trifold;

        _navigator.UpdateTitle(_path, Trifold.Title!);

        return Task.CompletedTask;
    }
}