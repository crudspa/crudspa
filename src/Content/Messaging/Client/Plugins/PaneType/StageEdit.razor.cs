namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class StageEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IStageService StageService { get; set; } = null!;
    public StageEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(Path, Id, IsNew, Path!.Id("campaign"), EventBus, Navigator, StageService);
        Model.PropertyChanged += HandleModelChanged;
        await Model.Refresh();
    }

    public void Dispose() { Model.PropertyChanged -= HandleModelChanged; Model.Dispose(); }
    private async Task HandleCancelClicked() { if (Model.IsNew) Navigator.Close(Path); else await Model.Refresh(); }
}

public class StageEditModel : EditModel<Stage>,
    IHandle<StageSaved>, IHandle<StageRemoved>, IHandle<StagesReordered>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _campaignId;
    private readonly INavigator _navigator;
    private readonly IStageService _stageService;

    public String? SendTimeText { get; set { if (SetProperty(ref field, value) && Entity is not null && TimeOnly.TryParse(value, out var time)) Entity.SendTime = time; } }

    public StageEditModel(String? path, Guid? id, Boolean isNew, Guid? campaignId,
        IEventBus eventBus, INavigator navigator, IStageService stageService) : base(isNew)
    {
        _path = path;
        _id = id;
        _campaignId = campaignId;
        _navigator = navigator;
        _stageService = stageService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(StageSaved payload) { if (payload.Id.Equals(_id)) await Refresh(); }
    public Task Handle(StageRemoved payload) { if (payload.Id.Equals(_id)) _navigator.Close(_path); return Task.CompletedTask; }
    public async Task Handle(StagesReordered payload) { if (!IsNew) await Refresh(); }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;
            SetStage(new()
            {
                CampaignId = _campaignId,
                Name = "New Stage",
                Anchor = Stage.Anchors.LessonStart,
                Offset = 0,
                WeekendAdjustment = Stage.WeekendAdjustments.NextWeekday,
                SendTime = new(9, 0),
            });
        }
        else
        {
            ReadOnly = true;
            var response = await WithWaiting("Fetching...", () => _stageService.Fetch(new(new() { Id = _id })));
            if (response.Ok) SetStage(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _stageService.Add(new(Entity!)));
            if (response.Ok) { _navigator.GoTo($"{_path.Parent()}/stage-{response.Value.Id:D}"); _navigator.Close(_path); }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _stageService.Save(new(Entity!)));
            if (response.Ok) ReadOnly = true;
        }
    }

    private void SetStage(Stage stage) { Entity = stage; SendTimeText = stage.SendTime?.ToString("t"); _navigator.UpdateTitle(_path, Entity.Name); }
}