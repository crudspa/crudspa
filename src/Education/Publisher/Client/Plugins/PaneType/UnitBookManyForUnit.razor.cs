namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class UnitBookManyForUnit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IUnitBookService UnitBookService { get; set; } = null!;

    public UnitBookManyForUnitModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, UnitBookService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class UnitBookManyForUnitModel : ManyOrderablesModel<UnitBookModel>,
    IHandle<UnitBookAdded>, IHandle<UnitBookSaved>, IHandle<UnitBookRemoved>, IHandle<UnitBooksReordered>,
    IHandle<BookAdded>, IHandle<BookSaved>, IHandle<BookRemoved>
{
    private readonly IUnitBookService _unitBookService;
    private readonly Guid? _unitId;

    public UnitBookManyForUnitModel(IEventBus eventBus, IScrollService scrollService, IUnitBookService unitBookService, Guid? unitId)
        : base(scrollService)
    {
        _unitBookService = unitBookService;

        _unitId = unitId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(UnitBookAdded payload) => await Replace(payload.Id, payload.UnitId);

    public async Task Handle(UnitBookSaved payload) => await Replace(payload.Id, payload.UnitId);

    public async Task Handle(UnitBookRemoved payload) => await Rid(payload.Id, payload.UnitId);

    public async Task Handle(UnitBooksReordered payload) => await Refresh();

    public async Task Handle(BookAdded payload) => await FetchBookNames();

    public async Task Handle(BookSaved payload) => await FetchBookNames();

    public async Task Handle(BookRemoved payload) => await FetchBookNames();

    public ObservableCollection<Named> BookNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchBookNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Unit>(new() { Id = _unitId });
        var response = await WithWaiting("Fetching...", () => _unitBookService.FetchForUnit(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new UnitBookModel(x)));
    }

    public override async Task Create()
    {
        var unitBook = new UnitBook
        {
            Id = Guid.NewGuid(),
            BookId = BookNames.FirstOrDefault()?.Id,
            Treatment = true,
            Control = true,
            Ordinal = Forms.Count,
            UnitId = _unitId,
        };

        var form = await CreateForm(new(unitBook));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_unitId);
    }

    public override async Task<Response<UnitBookModel?>> Fetch(Guid? id)
    {
        var response = await _unitBookService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new UnitBookModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<UnitBookModel?>> Add(FormModel<UnitBookModel> form)
    {
        var response = await _unitBookService.Add(new(form.Entity.UnitBook));

        return response.Ok
            ? new(new UnitBookModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<UnitBookModel> form)
    {
        var unitBook = form.Entity.UnitBook;

        return await _unitBookService.Save(new(unitBook));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _unitBookService.Remove(new(new()
        {
            Id = id,
            UnitId = _unitId,
        }));
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Forms.Select(x => x.Entity.UnitBook).ToList();
        return await WithWaiting("Saving...", () => _unitBookService.SaveOrder(new(orderables)));
    }

    public async Task FetchBookNames()
    {
        var response = await WithAlerts(() => _unitBookService.FetchBookNames(new()), false);
        if (response.Ok) BookNames = response.Value.ToObservable();
    }
}

public class UnitBookModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleUnitBookChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(UnitBook));

    private UnitBook _unitBook;

    public String? Name => UnitBook.BookTitle;

    public UnitBookModel(UnitBook unitBook)
    {
        _unitBook = unitBook;
        _unitBook.PropertyChanged += HandleUnitBookChanged;
    }

    public void Dispose()
    {
        _unitBook.PropertyChanged -= HandleUnitBookChanged;
    }

    public Guid? Id
    {
        get => _unitBook.Id;
        set => _unitBook.Id = value;
    }

    public Int32? Ordinal
    {
        get => _unitBook.Ordinal;
        set => _unitBook.Ordinal = value;
    }

    public UnitBook UnitBook
    {
        get => _unitBook;
        set => SetProperty(ref _unitBook, value);
    }
}