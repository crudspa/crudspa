namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class MovieEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IMovieService MovieService { get; set; } = null!;

    public MovieEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, EventBus, Navigator, MovieService);
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

public class MovieEditModel : EditModel<Movie>,
    IHandle<MovieSaved>, IHandle<MovieRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly IMovieService _movieService;

    public MovieEditModel(String? path, Guid? id, Boolean isNew,
        IEventBus eventBus,
        INavigator navigator,
        IMovieService movieService) : base(isNew)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _movieService = movieService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(MovieSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(MovieRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public List<Orderable> GenreNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> RatingNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchGenreNames(),
            FetchRatingNames());

        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var movie = new Movie
            {
                Title = "New Movie",
                GenreId = GenreNames.MinBy(x => x.Ordinal)?.Id,
                RatingId = RatingNames.MinBy(x => x.Ordinal)?.Id,
            };

            SetMovie(movie);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _movieService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetMovie(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _movieService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/movie-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _movieService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    public async Task FetchGenreNames()
    {
        var response = await WithAlerts(() => _movieService.FetchGenreNames(new()), false);
        if (response.Ok) GenreNames = response.Value.ToList();
    }

    public async Task FetchRatingNames()
    {
        var response = await WithAlerts(() => _movieService.FetchRatingNames(new()), false);
        if (response.Ok) RatingNames = response.Value.ToList();
    }

    private void SetMovie(Movie movie)
    {
        Entity = movie;
        _navigator.UpdateTitle(_path, Entity.Name);
    }
}