namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ModuleListForBook : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IModuleService ModuleService { get; set; } = null!;

    public ModuleListForBookModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ModuleService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ModuleListForBookModel : ListOrderablesModel<ModuleModel>,
    IHandle<ModuleAdded>, IHandle<ModuleSaved>, IHandle<ModuleRemoved>, IHandle<ModulesReordered>
{
    private readonly IModuleService _moduleService;
    private readonly Guid? _bookId;

    public ModuleListForBookModel(IEventBus eventBus, IScrollService scrollService, IModuleService moduleService, Guid? bookId)
        : base(scrollService)
    {
        _moduleService = moduleService;
        _bookId = bookId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ModuleAdded payload) => await Replace(payload.Id, payload.BookId);

    public async Task Handle(ModuleSaved payload) => await Replace(payload.Id, payload.BookId);

    public async Task Handle(ModuleRemoved payload) => await Rid(payload.Id, payload.BookId);

    public async Task Handle(ModulesReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Book>(new() { Id = _bookId });
        var response = await WithWaiting("Fetching...", () => _moduleService.FetchForBook(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ModuleModel(x)).ToList());
    }

    public override async Task<Response<ModuleModel?>> Fetch(Guid? id)
    {
        var response = await _moduleService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ModuleModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _moduleService.Remove(new(new()
        {
            Id = id,
            BookId = _bookId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_bookId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.Module).ToList();
        return await WithWaiting("Saving...", () => _moduleService.SaveOrder(new(orderables)));
    }
}

public class ModuleModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleModuleChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Module));

    private Module _module;

    public String? Name => Module.Title;

    public ModuleModel(Module module)
    {
        _module = module;
        _module.PropertyChanged += HandleModuleChanged;
    }

    public void Dispose()
    {
        _module.PropertyChanged -= HandleModuleChanged;
    }

    public Guid? Id
    {
        get => _module.Id;
        set => _module.Id = value;
    }

    public Int32? Ordinal
    {
        get => _module.Ordinal;
        set => _module.Ordinal = value;
    }

    public Module Module
    {
        get => _module;
        set => SetProperty(ref _module, value);
    }
}