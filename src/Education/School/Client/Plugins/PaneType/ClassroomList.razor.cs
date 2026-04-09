namespace Crudspa.Education.School.Client.Plugins.PaneType;

public partial class ClassroomList : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IClassroomService ClassroomService { get; set; } = null!;

    public ClassroomListModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ClassroomService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ClassroomListModel : ListModel<ClassroomModel>,
    IHandle<ClassroomAdded>, IHandle<ClassroomSaved>, IHandle<ClassroomRemoved>
{
    private readonly IClassroomService _classroomService;

    public ClassroomListModel(IEventBus eventBus, IScrollService scrollService, IClassroomService classroomService)
        : base(scrollService)
    {
        _classroomService = classroomService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ClassroomAdded payload) => await Replace(payload.Id);
    public async Task Handle(ClassroomSaved payload) => await Replace(payload.Id);
    public async Task Handle(ClassroomRemoved payload) => await Rid(payload.Id);

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(Guid.Empty);
        var response = await WithWaiting("Fetching...", () => _classroomService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetCards(response.Value.Select(x => new ClassroomModel(x)).ToList());
    }

    public override async Task<Response<ClassroomModel?>> Fetch(Guid? id)
    {
        var response = await _classroomService.Fetch(new(new() { Id = id }));

        if (!response.Ok)
            return new() { Errors = response.Errors };

        return new(new ClassroomModel(response.Value));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _classroomService.Remove(new(new() { Id = id }));
    }
}

public class ClassroomModel : Observable, IDisposable, INamed
{
    private Classroom _classroom;

    public String? Name => Classroom.Name;

    public ClassroomModel(Classroom classroom)
    {
        _classroom = classroom;
        _classroom.PropertyChanged += HandleClassroomChanged;
    }

    public void Dispose()
    {
        _classroom.PropertyChanged -= HandleClassroomChanged;
    }

    private void HandleClassroomChanged(Object? sender, PropertyChangedEventArgs args)
    {
        RaisePropertyChanged(nameof(Classroom));
    }

    public Guid? Id
    {
        get => _classroom.Id;
        set => _classroom.Id = value;
    }

    public Classroom Classroom
    {
        get => _classroom;
        set => SetProperty(ref _classroom, value);
    }
}