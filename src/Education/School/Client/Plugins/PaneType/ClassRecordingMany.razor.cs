namespace Crudspa.Education.School.Client.Plugins.PaneType;

public partial class ClassRecordingMany : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IClassRecordingService ClassRecordingService { get; set; } = null!;

    public ClassRecordingManyModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ClassRecordingService, SessionState);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ClassRecordingManyModel : ManyModel<ClassRecordingModel>,
    IHandle<ClassRecordingAdded>, IHandle<ClassRecordingSaved>, IHandle<ClassRecordingRemoved>
{
    private readonly IClassRecordingService _classRecordingService;
    private readonly ISessionState _sessionState;

    public ClassRecordingManyModel(IEventBus eventBus, IScrollService scrollService, IClassRecordingService classRecordingService, ISessionState sessionState)
        : base(scrollService)
    {
        _classRecordingService = classRecordingService;
        _sessionState = sessionState;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ClassRecordingAdded payload) => await Replace(payload.Id);
    public async Task Handle(ClassRecordingSaved payload) => await Replace(payload.Id);
    public async Task Handle(ClassRecordingRemoved payload) => await Rid(payload.Id, null);

    public List<Orderable> ContentCategoryNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchContentCategoryNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(new());
        var response = await WithWaiting("Fetching...", () => _classRecordingService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new ClassRecordingModel(x, _sessionState)));
    }

    public override async Task Create()
    {
        var classRecording = new ClassRecording
        {
            Id = Guid.NewGuid(),
            CategoryId = ContentCategoryNames.MinBy(x => x.Ordinal)?.Id,
        };

        await CreateForm(new(classRecording, _sessionState));
    }

    public override async Task<Response<ClassRecordingModel?>> Fetch(Guid? id)
    {
        var response = await _classRecordingService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ClassRecordingModel(response.Value, _sessionState))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<ClassRecordingModel?>> Add(FormModel<ClassRecordingModel> form)
    {
        var response = await _classRecordingService.Add(new(form.Entity.ClassRecording));

        return response.Ok
            ? new(new ClassRecordingModel(response.Value, _sessionState))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<ClassRecordingModel> form)
    {
        var classRecording = form.Entity.ClassRecording;

        return await _classRecordingService.Save(new(classRecording));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _classRecordingService.Remove(new(new()
        {
            Id = id,
        }));
    }

    public async Task FetchContentCategoryNames()
    {
        var response = await WithAlerts(() => _classRecordingService.FetchContentCategoryNames(new()), false);
        if (response.Ok) ContentCategoryNames = response.Value.ToList();
    }
}

public class ClassRecordingModel : Observable, IDisposable, INamed
{
    private void HandleClassRecordingChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ClassRecording));

    private ClassRecording _classRecording;
    private readonly ISessionState _sessionState;

    public String Name => ClassRecording.Uploaded.ToLocalTime(_sessionState.TimeZoneId).GetValueOrDefault(DateTimeOffset.Now).ToString("g");

    public ClassRecordingModel(ClassRecording classRecording, ISessionState sessionState)
    {
        _classRecording = classRecording;
        _sessionState = sessionState;
        _classRecording.PropertyChanged += HandleClassRecordingChanged;
    }

    public void Dispose()
    {
        _classRecording.PropertyChanged -= HandleClassRecordingChanged;
    }

    public Guid? Id
    {
        get => _classRecording.Id;
        set => _classRecording.Id = value;
    }

    public ClassRecording ClassRecording
    {
        get => _classRecording;
        set => SetProperty(ref _classRecording, value);
    }
}