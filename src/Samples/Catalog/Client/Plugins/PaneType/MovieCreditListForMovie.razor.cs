namespace Crudspa.Samples.Catalog.Client.Plugins.PaneType;

public partial class MovieCreditListForMovie : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMovieCreditService MovieCreditService { get; set; } = null!;

    public MovieCreditListForMovieModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MovieCreditService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class MovieCreditListForMovieModel : ListOrderablesModel<MovieCreditModel>,
    IHandle<MovieCreditAdded>, IHandle<MovieCreditSaved>, IHandle<MovieCreditRemoved>, IHandle<MovieCreditsReordered>
{
    private readonly IMovieCreditService _movieCreditService;
    private readonly Guid? _movieId;

    public MovieCreditListForMovieModel(IEventBus eventBus, IScrollService scrollService, IMovieCreditService movieCreditService, Guid? movieId)
        : base(scrollService)
    {
        _movieCreditService = movieCreditService;

        _movieId = movieId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(MovieCreditAdded payload) => await Replace(payload.Id, payload.MovieId);

    public async Task Handle(MovieCreditSaved payload) => await Replace(payload.Id, payload.MovieId);

    public async Task Handle(MovieCreditRemoved payload) => await Rid(payload.Id, payload.MovieId);

    public async Task Handle(MovieCreditsReordered payload) => await Refresh();

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<Movie>(new() { Id = _movieId });
        var response = await WithWaiting("Fetching...", () => _movieCreditService.FetchForMovie(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new MovieCreditModel(x)).ToList());
    }

    public override async Task<Response<MovieCreditModel?>> Fetch(Guid? id)
    {
        var response = await _movieCreditService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new MovieCreditModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _movieCreditService.Remove(new(new()
        {
            Id = id,
            MovieId = _movieId,
        }));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_movieId);
    }

    public override async Task<Response> SaveOrder()
    {
        var orderables = Cards.Select(x => x.Entity.MovieCredit).ToList();
        return await WithWaiting("Saving...", () => _movieCreditService.SaveOrder(new(orderables)));
    }
}

public class MovieCreditModel : Observable, IDisposable, INamed, IOrderable
{
    private void HandleMovieCreditChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(MovieCredit));

    private MovieCredit _movieCredit;

    public String? Name => MovieCredit.Name;

    public MovieCreditModel(MovieCredit movieCredit)
    {
        _movieCredit = movieCredit;
        _movieCredit.PropertyChanged += HandleMovieCreditChanged;
    }

    public void Dispose()
    {
        _movieCredit.PropertyChanged -= HandleMovieCreditChanged;
    }

    public Guid? Id
    {
        get => _movieCredit.Id;
        set => _movieCredit.Id = value;
    }

    public Int32? Ordinal
    {
        get => _movieCredit.Ordinal;
        set => _movieCredit.Ordinal = value;
    }

    public MovieCredit MovieCredit
    {
        get => _movieCredit;
        set => SetProperty(ref _movieCredit, value);
    }
}