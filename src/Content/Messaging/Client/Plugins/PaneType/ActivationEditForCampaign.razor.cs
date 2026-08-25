namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class ActivationEditForCampaign : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ICampaignService CampaignService { get; set; } = null!;
    [Inject] public IStageService StageService { get; set; } = null!;
    [Inject] public IActivationService ActivationService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ActivationEditForCampaignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var campaignId = Path?.Id("campaign") ?? Id;
        Model = new(Path, campaignId, Navigator, CampaignService, StageService, ActivationService, ScrollService);
        Model.PropertyChanged += HandleModelChanged;
        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ActivationEditForCampaignModel(
    String? path,
    Guid? campaignId,
    INavigator navigator,
    ICampaignService campaignService,
    IStageService stageService,
    IActivationService activationService,
    IScrollService scrollService) : ScreenModel
{
    public Campaign? Campaign { get; set => SetProperty(ref field, value); }
    public ObservableCollection<Named> Districts { get; set => SetProperty(ref field, value); } = [];
    public Guid? OrganizationId { get; set => SetProperty(ref field, value); }
    public ObservableCollection<Named> Statuses { get; } =
    [
        new() { Id = ActivationStatusIds.Scheduled, Name = "Scheduled" },
    ];
    public Guid? StatusId { get; } = ActivationStatusIds.Scheduled;
    public ObservableCollection<CampaignStageScheduleModel> Schedules { get; set => SetProperty(ref field, value); } = [];
    public DateOnly? Start { get; set => SetProperty(ref field, value); } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? LessonStart { get; set => SetProperty(ref field, value); } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? AssessmentStart { get; set => SetProperty(ref field, value); } = DateOnly.FromDateTime(DateTime.Today);
    public ObservableCollection<CampaignScheduleOption> ScheduleOptions { get; set => SetProperty(ref field, value); } = [];
    public ObservableCollection<Selectable> SelectableScheduleOptions { get; set => SetProperty(ref field, value); } = [];
    public ObservableCollection<CampaignScopeSchedule> Overrides { get; set => SetProperty(ref field, value); } = [];
    public ModalModel AddOverridesModel { get; } = new(scrollService);
    public Guid BatchId { get; } = Guid.NewGuid();
    public Boolean CanActivate => OrganizationId.HasValue && Schedules.HasItems();
    public Boolean CanAddOverrides => OrganizationId.HasValue && ScheduleOptions.HasItems();

    public async Task Initialize()
    {
        await WithMany("Loading campaign...", FetchCampaign(), FetchStages());
        navigator.UpdateTitle(path, Campaign is null ? "Activate Campaign" : $"Activate {Campaign.Name}");

        if (Campaign?.PortalId is not null)
            await FetchDistricts();
    }

    private async Task FetchCampaign()
    {
        var response = await WithAlerts(() => campaignService.Fetch(new(new() { Id = campaignId })), false);
        if (response.Ok) Campaign = response.Value;
    }

    private async Task FetchStages()
    {
        var response = await WithAlerts(() => stageService.FetchForCampaign(new(new() { Id = campaignId })), false);
        if (!response.Ok) return;
        Schedules = response.Value.OrderBy(x => x.Ordinal).Select(x => new CampaignStageScheduleModel(x)).ToObservable();
        Recalculate();
        RaisePropertyChanged(nameof(CanActivate));
    }

    private async Task FetchDistricts()
    {
        var response = await WithWaiting("Loading districts...", () => activationService.SearchTargets(new(new()
        {
            PortalId = Campaign!.PortalId,
            CampaignId = Campaign.Id,
        })));
        if (!response.Ok) return;

        Districts = response.Value
            .Where(x => !x.ParentOrganizationId.HasValue)
            .OrderBy(x => x.Name)
            .Select(x => new Named { Id = x.OrganizationId, Name = x.Name })
            .ToObservable();

        if (!OrganizationId.HasValue) await SetDistrict(Districts.FirstOrDefault()?.Id);
    }

    public async Task SetDistrict(Guid? organizationId)
    {
        if (OrganizationId == organizationId) return;
        OrganizationId = organizationId;
        Overrides.Clear();
        ScheduleOptions.Clear();

        if (organizationId.HasValue)
        {
            var response = await WithWaiting("Loading overrides...", () =>
                activationService.FetchScheduleOptions(new(new() { OrganizationId = organizationId.Value })));
            if (response.Ok) ScheduleOptions = response.Value.ToObservable();
        }

        RaisePropertyChanged(nameof(CanActivate));
        RaisePropertyChanged(nameof(CanAddOverrides));
    }

    public Task SetStart(DateOnly? start)
    {
        var prior = Start;
        Start = start;
        if (prior.HasValue && start.HasValue)
        {
            var days = start.Value.DayNumber - prior.Value.DayNumber;
            if (LessonStart.HasValue) LessonStart = LessonStart.Value.AddDays(days);
            if (AssessmentStart.HasValue) AssessmentStart = AssessmentStart.Value.AddDays(days);
        }
        Recalculate();
        RefreshRootSchedules();
        return Task.CompletedTask;
    }

    public Task SetLessonStart(DateOnly? start)
    {
        LessonStart = start;
        Recalculate();
        RefreshRootSchedules();
        return Task.CompletedTask;
    }

    public Task SetAssessmentStart(DateOnly? start)
    {
        AssessmentStart = start;
        Recalculate();
        RefreshRootSchedules();
        return Task.CompletedTask;
    }

    public Task SetStage(CampaignStageScheduleModel schedule, DateOnly? date)
    {
        schedule.SetCustom(date);
        RefreshRootSchedules();
        return Task.CompletedTask;
    }

    public async Task ShowAddOverrides()
    {
        SelectableScheduleOptions = ScheduleOptions.Select(x => new Selectable
        {
            Id = x.Id,
            Name = ScheduleOptionName(x),
        }).ToObservable();
        await AddOverridesModel.Show();
    }

    public void AddSelectedOverrides()
    {
        var selected = SelectableScheduleOptions.Where(x => x.Selected == true).Select(x => x.Id).ToHashSet();
        foreach (var option in ScheduleOptions.Where(x => selected.Contains(x.Id)).ToList()) AddOverride(option);
        AddOverridesModel.Hide();
        RaisePropertyChanged(nameof(CanAddOverrides));
    }

    private CampaignScopeSchedule? AddOverride(CampaignScheduleOption option)
    {
        if (!Start.HasValue || !LessonStart.HasValue || !AssessmentStart.HasValue || !OrganizationId.HasValue)
            return null;

        var parent = option.DistrictWide
            ? null
            : Overrides.FirstOrDefault(x => x.OrganizationId == OrganizationId && x.GradeId == option.GradeId);
        var scope = new CampaignScopeSchedule
        {
            ParentKey = parent?.Key,
            DistrictOrganizationId = OrganizationId.Value,
            OrganizationId = option.OrganizationId,
            OrganizationName = option.OrganizationName,
            GradeId = option.GradeId,
            GradeName = option.GradeName,
            Start = parent?.Start ?? Start,
            LessonStart = parent?.LessonStart ?? LessonStart,
            AssessmentStart = parent?.AssessmentStart ?? AssessmentStart,
            Schedules = Schedules.Select(x => new StageSchedule
            {
                StageId = x.Stage.Id,
                LocalSend = parent?.Schedules.First(y => y.StageId == x.Stage.Id).LocalSend ??
                    x.Date?.ToDateTime(x.Stage.SendTime ?? TimeOnly.MinValue),
            }).ToList(),
        };
        Overrides.Add(scope);
        if (option.DistrictWide)
        {
            foreach (var school in Overrides.Where(x =>
                         x.Key != scope.Key && x.OrganizationId != x.DistrictOrganizationId && x.GradeId == scope.GradeId))
            {
                school.ParentKey = scope.Key;
                RefreshInherited(school);
            }
        }
        ScheduleOptions.Remove(option);
        return scope;
    }

    public Task RemoveOverride(Guid? id)
    {
        var scope = Overrides.FirstOrDefault(x => x.Key == id);
        if (scope is null) return Task.CompletedTask;
        foreach (var child in Overrides.Where(x => x.ParentKey == scope.Key).ToList())
        {
            child.ParentKey = scope.ParentKey;
            RefreshInherited(child);
        }
        Overrides.Remove(scope);
        ScheduleOptions.Add(new()
        {
            Id = Guid.NewGuid(),
            DistrictOrganizationId = scope.DistrictOrganizationId,
            OrganizationId = scope.OrganizationId,
            OrganizationName = scope.OrganizationName,
            GradeId = scope.GradeId,
            GradeName = scope.GradeName,
        });
        ScheduleOptions = ScheduleOptions.OrderBy(x => x.DistrictWide ? 0 : 1)
            .ThenBy(x => x.GradeName).ThenBy(x => x.OrganizationName).ToObservable();
        RaisePropertyChanged(nameof(CanAddOverrides));
        return Task.CompletedTask;
    }

    public String OverrideName(CampaignScopeSchedule scope) => scope.OrganizationId == scope.DistrictOrganizationId
        ? scope.GradeName ?? String.Empty
        : $"{scope.OrganizationName} | {scope.GradeName}";

    private static String ScheduleOptionName(CampaignScheduleOption option) => option.DistrictWide
        ? option.GradeName ?? String.Empty
        : $"{option.OrganizationName} | {option.GradeName}";

    public Task SetScopeStart(CampaignScopeSchedule scope, DateOnly? start)
    {
        scope.Start = start;
        scope.StartOverridden = true;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public Task SetScopeLessonStart(CampaignScopeSchedule scope, DateOnly? start)
    {
        scope.LessonStart = start;
        scope.LessonStartOverridden = true;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public Task SetScopeAssessmentStart(CampaignScopeSchedule scope, DateOnly? start)
    {
        scope.AssessmentStart = start;
        scope.AssessmentStartOverridden = true;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public Task SetScopeStage(CampaignScopeSchedule scope, StageSchedule schedule, DateOnly? date)
    {
        var stage = Schedules.Single(x => x.Stage.Id == schedule.StageId).Stage;
        schedule.LocalSend = date?.ToDateTime(stage.SendTime ?? TimeOnly.MinValue);
        schedule.Overridden = true;
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public OverrideState ScopeStartState(CampaignScopeSchedule scope) =>
        scope.StartOverridden ? OverrideState.Custom : OverrideState.Default;
    public OverrideState ScopeLessonStartState(CampaignScopeSchedule scope) =>
        scope.LessonStartOverridden ? OverrideState.Custom : OverrideState.Default;
    public OverrideState ScopeAssessmentStartState(CampaignScopeSchedule scope) =>
        scope.AssessmentStartOverridden ? OverrideState.Custom : OverrideState.Default;
    public OverrideState ScopeStageState(StageSchedule schedule) =>
        schedule.Overridden ? OverrideState.Custom : OverrideState.Default;

    public Task SetScopeStartState(CampaignScopeSchedule scope, OverrideState state)
    {
        scope.StartOverridden = state == OverrideState.Custom;
        if (!scope.StartOverridden) scope.Start = Parent(scope)?.Start ?? Start;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public Task SetScopeLessonStartState(CampaignScopeSchedule scope, OverrideState state)
    {
        scope.LessonStartOverridden = state == OverrideState.Custom;
        if (!scope.LessonStartOverridden) scope.LessonStart = Parent(scope)?.LessonStart ?? LessonStart;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public Task SetScopeAssessmentStartState(CampaignScopeSchedule scope, OverrideState state)
    {
        scope.AssessmentStartOverridden = state == OverrideState.Custom;
        if (!scope.AssessmentStartOverridden) scope.AssessmentStart = Parent(scope)?.AssessmentStart ?? AssessmentStart;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public Task SetScopeStageState(CampaignScopeSchedule scope, StageSchedule schedule, OverrideState state)
    {
        schedule.Overridden = state == OverrideState.Custom;
        if (!schedule.Overridden) schedule.LocalSend = DerivedStage(scope, schedule.StageId);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public void Reset(CampaignStageScheduleModel schedule)
    {
        schedule.Overridden = false;
        SetDerived(schedule);
        RefreshRootSchedules();
    }

    private void Recalculate()
    {
        foreach (var schedule in Schedules.Where(x => !x.Overridden)) SetDerived(schedule);
    }

    private void SetDerived(CampaignStageScheduleModel schedule)
    {
        schedule.SetDerived(CampaignScheduleCalculator.CalculateDate(
            schedule.Stage.Anchor, schedule.Stage.Offset, schedule.Stage.WeekendAdjustment,
            Start, LessonStart, AssessmentStart));
    }

    private CampaignScopeSchedule? Parent(CampaignScopeSchedule scope) =>
        Overrides.FirstOrDefault(x => x.Key == scope.ParentKey);

    private void RefreshRootSchedules()
    {
        foreach (var scope in Overrides.Where(x => !x.ParentKey.HasValue)) RefreshInherited(scope);
    }

    private void RefreshInherited(CampaignScopeSchedule scope)
    {
        var parent = Parent(scope);
        if (!scope.StartOverridden) scope.Start = parent?.Start ?? Start;
        if (!scope.LessonStartOverridden) scope.LessonStart = parent?.LessonStart ?? LessonStart;
        if (!scope.AssessmentStartOverridden) scope.AssessmentStart = parent?.AssessmentStart ?? AssessmentStart;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
    }

    private void RefreshChildren(CampaignScopeSchedule scope)
    {
        foreach (var child in Overrides.Where(x => x.ParentKey == scope.Key)) RefreshInherited(child);
    }

    private void RefreshDerivedStages(CampaignScopeSchedule scope)
    {
        foreach (var schedule in scope.Schedules.Where(x => !x.Overridden))
            schedule.LocalSend = DerivedStage(scope, schedule.StageId);
    }

    private DateTime? DerivedStage(CampaignScopeSchedule scope, Guid? stageId)
    {
        var parent = Parent(scope);
        var schedule = Schedules.First(x => x.Stage.Id == stageId);
        var send = parent?.Schedules.First(x => x.StageId == stageId).LocalSend ??
            schedule.Date?.ToDateTime(schedule.Stage.SendTime ?? TimeOnly.MinValue);
        var parentAnchor = CampaignScheduleCalculator.GetAnchor(schedule.Stage.Anchor,
            parent?.Start ?? Start, parent?.LessonStart ?? LessonStart, parent?.AssessmentStart ?? AssessmentStart);
        var anchor = CampaignScheduleCalculator.GetAnchor(schedule.Stage.Anchor,
            scope.Start, scope.LessonStart, scope.AssessmentStart);
        if (!send.HasValue || !parentAnchor.HasValue || !anchor.HasValue) return send;
        return send.Value.AddDays(anchor.Value.DayNumber - parentAnchor.Value.DayNumber);
    }

    public async Task Activate()
    {
        var activation = new CampaignActivation
        {
            BatchId = BatchId,
            CampaignId = Campaign?.Id,
            OrganizationId = OrganizationId,
            Start = Start,
            LessonStart = LessonStart,
            AssessmentStart = AssessmentStart,
            Schedules = Schedules.Select(x => new StageSchedule
            {
                StageId = x.Stage.Id,
                LocalSend = x.Date?.ToDateTime(x.Stage.SendTime ?? TimeOnly.MinValue),
                Overridden = x.Overridden,
            }).ToList(),
            Overrides = Overrides.ToList(),
        };

        if (!IsValid(activation)) return;

        var response = await WithWaiting("Activating campaign...", () => activationService.Activate(new(activation)));
        if (response.Ok) await Done();
    }

    public Task Cancel() => Done();

    private Task Done()
    {
        var campaignPath = path.Parent();
        navigator.Close(path);
        navigator.GoTo($"{campaignPath}?pane=activations");
        return Task.CompletedTask;
    }
}

public class CampaignStageScheduleModel(Stage stage) : Observable
{
    public Stage Stage { get; } = stage;
    public DateOnly? Date { get; set => SetProperty(ref field, value); }
    public Boolean Overridden { get; set => SetProperty(ref field, value); }

    public void SetCustom(DateOnly? date)
    {
        Date = date;
        Overridden = true;
    }

    public void SetDerived(DateOnly? date) => Date = date;
}