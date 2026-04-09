namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class MovieCreditEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IMovieCreditService MovieCreditService { get; set; } = null!;

    public MovieCreditEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, MovieCreditService);
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

public class MovieCreditEditModel : EditModel<MovieCredit>,
    IHandle<MovieCreditSaved>, IHandle<MovieCreditRemoved>, IHandle<MovieCreditsReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IMovieCreditService _movieCreditService;

    public MovieCreditEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IMovieCreditService movieCreditService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _movieCreditService = movieCreditService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(MovieCreditSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(MovieCreditRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Handle(MovieCreditsReordered payload)
    {
        if (IsNew || Entity is null)
            return;

        var response = await _movieCreditService.Fetch(new(new() { Id = _id }));

        if (response.Ok)
        {
            Entity!.Ordinal = response.Value.Ordinal;
            _navigator.UpdateTitle(_path, Entity.Name);
        }
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var movieCredit = new MovieCredit
            {
                Name = "New Movie Credit",
                Part = String.Empty,
            };

            SetMovieCredit(movieCredit);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _movieCreditService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetMovieCredit(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _movieCreditService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/movie-credit-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _movieCreditService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }


    private void SetMovieCredit(MovieCredit movieCredit)
    {
        Entity = movieCredit;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}