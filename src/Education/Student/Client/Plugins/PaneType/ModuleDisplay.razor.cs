namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class ModuleDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public IModuleProgressService ModuleProgressService { get; set; } = null!;

    public ModuleDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, StudentAppService, ModuleProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ModuleDisplayModel : ScreenModel
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IStudentAppService _studentAppService;
    private readonly IModuleProgressService _moduleProgressService;
    private Module? _module;

    public ModuleDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        IStudentAppService studentAppService,
        IModuleProgressService moduleProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _studentAppService = studentAppService;
        _moduleProgressService = moduleProgressService;

        eventBus.Subscribe(this);
    }

    public Module? Module
    {
        get => _module;
        set => SetProperty(ref _module, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchModule(new(new() { Id = _id })));

        if (response.Ok)
            await SetModule(response.Value);
    }

    public async Task HandleBinderCompleted()
    {
        await _moduleProgressService.AddCompleted(new(new()
        {
            ModuleId = _module!.Id,
            DeviceTimestamp = DateTimeOffset.Now,
        }));

        _navigator.Close(_path);
    }

    private Task SetModule(Module module)
    {
        Module = module;

        _navigator.UpdateTitle(_path, Module.Title!);

        return Task.CompletedTask;
    }
}