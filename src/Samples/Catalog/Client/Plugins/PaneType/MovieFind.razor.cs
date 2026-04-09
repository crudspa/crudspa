namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class MovieFind : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMovieService MovieService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;

    public MovieFindModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MovieService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Reset();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void AddNew()
    {
        Navigator.GoTo($"{Path}/movie-{Guid.NewGuid():D}?state=new");
    }
}

public class MovieFindModel : FindModel<MovieSearch, Movie>,
    IHandle<MovieAdded>, IHandle<MovieSaved>, IHandle<MovieRemoved>,
    IHandle<MovieCreditAdded>, IHandle<MovieCreditRemoved>
{
    private readonly IMovieService _movieService;
    private ObservableCollection<String> _sorts;
    private Boolean _resetting;

    public MovieFindModel(IEventBus eventBus, IScrollService scrollService, IMovieService movieService)
        : base(scrollService)
    {
        _movieService = movieService;
        eventBus.Subscribe(this);

        _sorts =
        [
            "Title",
            "Released",
            "Score",
        ];
    }

    public async Task Handle(MovieAdded payload) => await Refresh();

    public async Task Handle(MovieSaved payload) => await Refresh();

    public async Task Handle(MovieRemoved payload) => await Refresh();

    public async Task Handle(MovieCreditAdded payload) => await Refresh();

    public async Task Handle(MovieCreditRemoved payload) => await Refresh();

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

    public ObservableCollection<String> Sorts
    {
        get => _sorts;
        set => SetProperty(ref _sorts, value);
    }

    public async Task Reset()
    {
        _resetting = true;

        Search.Text = String.Empty;

        Search.Paged.PageNumber = 1;
        Search.Paged.PageSize = 50;
        Search.Paged.TotalCount = 0;

        Search.Sort.Field = Sorts.First();
        Search.Sort.Ascending = true;
        Search.Genres.Clear();
        Search.Ratings.Clear();
        Search.ReleasedRange.Type = DateRange.Types.Any;

        await WithMany("Initializing...",
            FetchGenreNames(),
            FetchRatingNames());

        _resetting = false;

        await Refresh(false);
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        if (_resetting)
            return;

        var request = new Request<MovieSearch>(Search);
        var response = await WithWaiting("Searching...", () => _movieService.Search(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value);
    }

    public async Task Delete(Guid? id)
    {
        await WithWaiting("Deleting...", () => _movieService.Remove(new(new() { Id = id })));
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
}