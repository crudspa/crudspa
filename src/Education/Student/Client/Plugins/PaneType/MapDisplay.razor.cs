namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class MapDisplay : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISoundService SoundService { get; set; } = null!;
    [Inject] public IStudentAppService StudentAppService { get; set; } = null!;
    [Inject] public ITrifoldProgressService TrifoldProgressService { get; set; } = null!;

    public MapDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, SoundService, StudentAppService, TrifoldProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class MapDisplayModel : ScreenModel, IHandle<TrifoldProgressUpdated>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ISoundService _soundService;
    private readonly IStudentAppService _studentAppService;
    private readonly ITrifoldProgressService _trifoldProgressService;

    public MapDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        ISoundService soundService,
        IStudentAppService studentAppService,
        ITrifoldProgressService trifoldProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _soundService = soundService;
        _studentAppService = studentAppService;
        _trifoldProgressService = trifoldProgressService;

        eventBus.Subscribe(this);
    }

    public BookContent? BookContent
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchTrifolds(new(new() { Id = _id })));

        if (response.Ok)
            await SetBookContent(response.Value);
    }

    public Task Handle(TrifoldProgressUpdated payload)
    {
        if (BookContent is null)
            return Task.CompletedTask;

        foreach (var trifold in BookContent.Trifolds)
        {
            if (trifold.Id.Equals(payload.Progress.TrifoldId))
            {
                trifold.Progress = payload.Progress;
                RaisePropertyChanged(nameof(BookContent));
                break;
            }
        }

        return Task.CompletedTask;
    }

    public void GoToTrifold(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/trifold-{id:D}");
    }

    private async Task SetBookContent(BookContent bookContent)
    {
        foreach (var trifold in bookContent.Trifolds)
            trifold.Progress = await _trifoldProgressService.Fetch(new(new() { Id = trifold.Id }));

        BookContent = bookContent;

        _navigator.UpdateTitle(_path, BookContent.Title!);
    }
}