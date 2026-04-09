namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class ShirtEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IShirtService ShirtService { get; set; } = null!;

    public ShirtEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, ShirtService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class ShirtEditModel : EditModel<Shirt>,
    IHandle<ShirtSaved>, IHandle<ShirtRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IShirtService _shirtService;

    public ShirtEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IShirtService shirtService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _shirtService = shirtService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ShirtSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(ShirtRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<Orderable> BrandNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchBrandNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var shirt = new Shirt
            {
                Name = "New Shirt",
                BrandId = BrandNames.MinBy(x => x.Ordinal)?.Id,
            };

            SetShirt(shirt);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _shirtService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetShirt(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _shirtService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/shirt-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _shirtService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchBrandNames()
    {
        var response = await WithAlerts(() => _shirtService.FetchBrandNames(new()), false);
        if (response.Ok) BrandNames = response.Value.ToList();
    }

    private void SetShirt(Shirt shirt)
    {
        Entity = shirt;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}