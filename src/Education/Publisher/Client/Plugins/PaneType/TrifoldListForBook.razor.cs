namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class TrifoldListForBook : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ITrifoldService TrifoldService { get; set; } = null!;

    public TrifoldListForBookModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, TrifoldService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class TrifoldListForBookModel : ListOrderablesModel<TrifoldModel>,
    IHandle<TrifoldAdded>, IHandle<TrifoldSaved>, IHandle<TrifoldRemoved>, IHandle<TrifoldsReordered>
{
    private readonly ITrifoldService _trifoldService;
    private readonly Guid? _bookId;

    public TrifoldListForBookModel(IEventBus eventBus, IScrollService scrollService, ITrifoldService trifoldService, Guid? bookId)
        : base(scrollService)
    {
        _trifoldService = trifoldService;

        _bookId = bookId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(TrifoldAdded payload) => await Replace(payload.Id, payload.BookId);

    public async Task Handle(TrifoldSaved payload) => await Replace(payload.Id, payload.BookId);

    public async Task Handle(TrifoldRemoved payload) => await Rid(payload.Id, payload.BookId);

    public async Task Handle(TrifoldsReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Book>(new() { Id = _bookId });
        var response = await WithWaiting("Fetching...", () => _trifoldService.FetchForBook(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new TrifoldModel(x)).ToList());
    }

    public override async Task<Response<TrifoldModel?>> Fetch(Guid? id)
    {
        var response = await _trifoldService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new TrifoldModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _trifoldService.Remove(new(new()
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
        var orderables = Cards.Select(x => x.Entity.Trifold).ToList();
        return await WithWaiting("Saving...", () => _trifoldService.SaveOrder(new(orderables)));
    }
}

public class TrifoldModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleTrifoldChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Trifold));

    private Trifold _trifold;

    public String? Name => Trifold.Title;

    public TrifoldModel(Trifold trifold)
    {
        _trifold = trifold;
        _trifold.PropertyChanged += HandleTrifoldChanged;
    }

    public void Dispose()
    {
        _trifold.PropertyChanged -= HandleTrifoldChanged;
    }

    public Guid? Id
    {
        get => _trifold.Id;
        set => _trifold.Id = value;
    }

    public Int32? Ordinal
    {
        get => _trifold.Ordinal;
        set => _trifold.Ordinal = value;
    }

    public Trifold Trifold
    {
        get => _trifold;
        set => SetProperty(ref _trifold, value);
    }
}