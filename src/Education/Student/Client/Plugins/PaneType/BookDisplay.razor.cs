using Crudspa.Content.Display.Client.Components;

namespace Crudspa.Education.Student.Client.Plugins.PaneType;

public partial class BookDisplay : IPaneDisplay, IDisposable
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
    [Inject] public IBookProgressService BookProgressService { get; set; } = null!;
    [Inject] public IGameProgressService GameProgressService { get; set; } = null!;
    [Inject] public IModuleProgressService ModuleProgressService { get; set; } = null!;

    public BookDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, EventBus, Navigator, SoundService, StudentAppService, BookProgressService, GameProgressService, ModuleProgressService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class BookDisplayModel : ScreenModel,
    IHandle<BookProgressUpdated>,
    IHandle<GameProgressUpdated>,
    IHandle<ModuleProgressUpdated>,
    IHandle<StudentAchievementAdded>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly INavigator _navigator;
    private readonly ISoundService _soundService;
    private readonly IStudentAppService _studentAppService;
    private readonly IBookProgressService _bookProgressService;
    private readonly IGameProgressService _gameProgressService;
    private readonly IModuleProgressService _moduleProgressService;

    public BookDisplayModel(String? path, Guid? id, IEventBus eventBus,
        INavigator navigator,
        ISoundService soundService,
        IStudentAppService studentAppService,
        IBookProgressService bookProgressService,
        IGameProgressService gameProgressService,
        IModuleProgressService moduleProgressService)
    {
        _path = path;
        _id = id;
        _navigator = navigator;
        _soundService = soundService;
        _studentAppService = studentAppService;
        _bookProgressService = bookProgressService;
        _gameProgressService = gameProgressService;
        _moduleProgressService = moduleProgressService;

        eventBus.Subscribe(this);
    }

    public BookLite? Book
    {
        get;
        set => SetProperty(ref field, value);
    }

    public GuideModel? GuideModel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Refresh()
    {
        var response = await WithWaiting("Loading...", () => _studentAppService.FetchBook(new(new() { Id = _id })));

        if (response.Ok)
        {
            var book = response.Value;

            book.Progress = await _bookProgressService.Fetch(new(new() { Id = book.Id }));

            foreach (var game in book.Games)
                game.Progress = await _gameProgressService.Fetch(new(game));

            foreach (var module in book.Modules)
                module.Progress = await _moduleProgressService.Fetch(new(module));

            SetBook(book);
        }
    }

    public Task Handle(BookProgressUpdated payload)
    {
        if (Book is null)
            return Task.CompletedTask;

        if (payload.Progress.BookId.Equals(Book.Id))
        {
            Book.Progress = payload.Progress;
            RaisePropertyChanged(nameof(Book));
        }

        return Task.CompletedTask;
    }

    public Task Handle(GameProgressUpdated payload)
    {
        foreach (var game in Book!.Games.Where(x => x.Id.Equals(payload.Progress.GameId)))
            game.Progress = payload.Progress;

        RaisePropertyChanged(nameof(Book));

        return Task.CompletedTask;
    }

    public Task Handle(ModuleProgressUpdated payload)
    {
        foreach (var module in Book!.Modules.Where(x => x.Id.Equals(payload.Progress.ModuleId)))
            module.Progress = payload.Progress;

        RaisePropertyChanged(nameof(Book));

        return Task.CompletedTask;
    }

    public async Task Handle(StudentAchievementAdded payload)
    {
        await Refresh();
    }

    public void GoToPreface()
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/preface-{Book!.Id:D}");
    }

    public void GoToContent()
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/content-{Book!.Id:D}");
    }

    public void GoToGame(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/game-{id:D}");
    }

    public void GoToMap()
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/map-{Book!.Id:D}");
    }

    public void GoToModule(Guid? id)
    {
        _soundService.ButtonPress();
        _navigator.GoTo($"{_path}/module-{id:D}");
    }

    private void SetBook(BookLite book)
    {
        Book = book;
        _navigator.UpdateTitle(_path, Book.Title!);
    }
}