namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class ShirtOptionManyForShirt : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IShirtOptionService ShirtOptionService { get; set; } = null!;

    public ShirtOptionManyForShirtModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ShirtOptionService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ShirtOptionManyForShirtModel : ManyOrderablesModel<ShirtOptionModel>,
    IHandle<ShirtOptionAdded>, IHandle<ShirtOptionSaved>, IHandle<ShirtOptionRemoved>, IHandle<ShirtOptionsReordered>, IHandle<ShirtOptionRelationsSaved>
{
    private readonly IShirtOptionService _shirtOptionService;
    private readonly Guid? _shirtId;

    public ShirtOptionManyForShirtModel(IEventBus eventBus, IScrollService scrollService, IShirtOptionService shirtOptionService, Guid? shirtId)
        : base(scrollService)
    {
        _shirtOptionService = shirtOptionService;

        _shirtId = shirtId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ShirtOptionAdded payload) => await Replace(payload.Id, payload.ShirtId);

    public async Task Handle(ShirtOptionSaved payload) => await Replace(payload.Id, payload.ShirtId);

    public async Task Handle(ShirtOptionRemoved payload) => await Rid(payload.Id, payload.ShirtId);

    public async Task Handle(ShirtOptionsReordered payload) => await Refresh();

    public async Task Handle(ShirtOptionRelationsSaved payload) => await Replace(payload.Id, payload.ShirtId);

    public List<Orderable> ColorNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchColorNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Shirt>(new() { Id = _shirtId });
        var response = await WithWaiting("Fetching...", () => _shirtOptionService.FetchForShirt(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new ShirtOptionModel(x)));
    }

    public override async Task Create()
    {
        var shirtOption = new ShirtOption
        {
            Id = Guid.NewGuid(),
            SkuBase = String.Empty,
            ColorId = ColorNames.MinBy(x => x.Ordinal)?.Id,
            AllSizes = true,
            Ordinal = Forms.Count,
            ShirtId = _shirtId,
        };

        var form = await CreateForm(new(shirtOption));
        form.Entity.SelectingColor = true;
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_shirtId);
    }

    public override async Task<Response<ShirtOptionModel?>> Fetch(Guid? id)
    {
        var response = await _shirtOptionService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ShirtOptionModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<ShirtOptionModel?>> Add(FormModel<ShirtOptionModel> form)
    {
        var response = await _shirtOptionService.Add(new(form.Entity.ShirtOption));

        return response.Ok
            ? new(new ShirtOptionModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<ShirtOptionModel> form)
    {
        var shirtOption = form.Entity.ShirtOption;

        if (!form.Entity.SelectingColor)
            return await _shirtOptionService.SaveRelations(new(shirtOption));

        return await _shirtOptionService.Save(new(shirtOption));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _shirtOptionService.Remove(new(new()
        {
            Id = id,
            ShirtId = _shirtId,
        }));
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Forms.Select(x => x.Entity.ShirtOption).ToList();
        return await WithWaiting("Saving...", () => _shirtOptionService.SaveOrder(new(orderables)));
    }

    public async Task FetchColorNames()
    {
        var response = await WithAlerts(() => _shirtOptionService.FetchColorNames(new()), false);
        if (response.Ok) ColorNames = response.Value.ToList();
    }
}

public class ShirtOptionModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleShirtOptionChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ShirtOption));

    private ShirtOption _shirtOption;
    private Guid? _previousColorId;

    public String? Name => ShirtOption.Name;

    public ShirtOptionModel(ShirtOption shirtOption)
    {
        _shirtOption = shirtOption;
        _shirtOption.PropertyChanged += HandleShirtOptionChanged;
    }

    public void Dispose()
    {
        _shirtOption.PropertyChanged -= HandleShirtOptionChanged;
    }

    public Guid? Id
    {
        get => _shirtOption.Id;
        set => _shirtOption.Id = value;
    }

    public Int32? Ordinal
    {
        get => _shirtOption.Ordinal;
        set => _shirtOption.Ordinal = value;
    }

    public ShirtOption ShirtOption
    {
        get => _shirtOption;
        set => SetProperty(ref _shirtOption, value);
    }

    public Boolean SelectingColor
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void StartColorSelection()
    {
        _previousColorId = ShirtOption.ColorId;
        SelectingColor = true;
    }

    public void CancelColorSelection()
    {
        ShirtOption.ColorId = _previousColorId;
        SelectingColor = false;
    }
}