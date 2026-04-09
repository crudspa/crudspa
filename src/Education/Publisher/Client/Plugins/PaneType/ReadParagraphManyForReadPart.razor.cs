namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class ReadParagraphManyForReadPart : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IReadParagraphService ReadParagraphService { get; set; } = null!;

    public ReadParagraphManyForReadPartModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ReadParagraphService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ReadParagraphManyForReadPartModel : ManyOrderablesModel<ReadParagraphModel>,
    IHandle<ReadParagraphAdded>, IHandle<ReadParagraphSaved>, IHandle<ReadParagraphRemoved>, IHandle<ReadParagraphsReordered>
{
    private readonly IReadParagraphService _readParagraphService;
    private readonly Guid? _readPartId;

    public ReadParagraphManyForReadPartModel(IEventBus eventBus, IScrollService scrollService, IReadParagraphService readParagraphService, Guid? readPartId)
        : base(scrollService)
    {
        _readParagraphService = readParagraphService;

        _readPartId = readPartId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ReadParagraphAdded payload) => await Replace(payload.Id, payload.ReadPartId);

    public async Task Handle(ReadParagraphSaved payload) => await Replace(payload.Id, payload.ReadPartId);

    public async Task Handle(ReadParagraphRemoved payload) => await Rid(payload.Id, payload.ReadPartId);

    public async Task Handle(ReadParagraphsReordered payload) => await Refresh();

    public async Task Initialize()
    {
        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<ReadPart>(new() { Id = _readPartId });
        var response = await WithWaiting("Fetching...", () => _readParagraphService.FetchForReadPart(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new ReadParagraphModel(x)));
    }

    public override async Task Create()
    {
        var readParagraph = new ReadParagraph
        {
            Id = Guid.NewGuid(),
            ReadPartId = _readPartId,
            Text = String.Empty,
            Ordinal = Forms.Count,
        };

        var form = await CreateForm(new(readParagraph));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_readPartId);
    }

    public override async Task<Response<ReadParagraphModel?>> Fetch(Guid? id)
    {
        var response = await _readParagraphService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ReadParagraphModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<ReadParagraphModel?>> Add(FormModel<ReadParagraphModel> form)
    {
        var response = await _readParagraphService.Add(new(form.Entity.ReadParagraph));

        return response.Ok
            ? new(new ReadParagraphModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<ReadParagraphModel> form)
    {
        var readParagraph = form.Entity.ReadParagraph;

        return await _readParagraphService.Save(new(readParagraph));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _readParagraphService.Remove(new(new()
        {
            Id = id,
            ReadPartId = _readPartId,
        }));
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Forms.Select(x => x.Entity.ReadParagraph).ToList();
        return await WithWaiting("Saving...", () => _readParagraphService.SaveOrder(new(orderables)));
    }
}

public class ReadParagraphModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleReadParagraphChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ReadParagraph));

    private ReadParagraph _readParagraph;

    public String Name => ReadParagraph.Name;

    public ReadParagraphModel(ReadParagraph readParagraph)
    {
        _readParagraph = readParagraph;
        _readParagraph.PropertyChanged += HandleReadParagraphChanged;
    }

    public void Dispose()
    {
        _readParagraph.PropertyChanged -= HandleReadParagraphChanged;
    }

    public Guid? Id
    {
        get => _readParagraph.Id;
        set => _readParagraph.Id = value;
    }

    public Int32? Ordinal
    {
        get => _readParagraph.Ordinal;
        set => _readParagraph.Ordinal = value;
    }

    public ReadParagraph ReadParagraph
    {
        get => _readParagraph;
        set => SetProperty(ref _readParagraph, value);
    }
}