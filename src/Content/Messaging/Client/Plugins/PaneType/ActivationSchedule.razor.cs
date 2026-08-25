namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class ActivationSchedule : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IActivationService ActivationService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public ActivationScheduleModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var activationId = Path?.Id("activation") ?? Id;
        Model = new(Path, activationId, ActivationService, Navigator, ScrollService);
        Model.PropertyChanged += HandleModelChanged;
        await Model.Fetch();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ActivationScheduleModel(
    String? path,
    Guid? activationId,
    IActivationService activationService,
    INavigator navigator,
    IScrollService scrollService) : ScreenModel
{
    public ModalModel AddSchedulesModel { get; } = new(scrollService);
    public CampaignScheduleConfiguration? Configuration { get; set => SetProperty(ref field, value); }
    public ObservableCollection<CampaignScheduleOption> AvailableOptions { get; set => SetProperty(ref field, value); } = [];
    public ObservableCollection<Selectable> SelectableScheduleOptions { get; set => SetProperty(ref field, value); } = [];
    public CampaignScopeConfiguration? Root => Configuration?.Scopes.SingleOrDefault(x => !x.ParentId.HasValue);
    public IList<CampaignScopeConfiguration> Overrides => Configuration?.Scopes
        .Where(x => x.ParentId.HasValue).OrderBy(x => x.Ordinal).ToList() ?? [];

    public async Task Fetch(Boolean resetAlerts = true)
    {
        var response = await WithWaiting("Loading campaign schedule...", () =>
            activationService.FetchSchedule(new(new() { Id = activationId })), resetAlerts);
        if (!response.Ok || response.Value is null) return;

        Configuration = response.Value;
        RefreshOptions();
        RaisePropertyChanged(nameof(Root));
        RaisePropertyChanged(nameof(Overrides));
        navigator.UpdateTitle(path, $"{Configuration.CampaignName} schedule");
    }

    private void RefreshOptions()
    {
        if (Configuration is null) return;
        AvailableOptions = Configuration.Options.Where(option => !Configuration.Scopes.Any(scope =>
            scope.OrganizationId == option.OrganizationId && scope.GradeId == option.GradeId)).ToObservable();
    }

    public String ScopeName(CampaignScopeConfiguration scope) => scope.OrganizationId == Configuration?.OrganizationId
        ? scope.GradeName ?? String.Empty
        : $"{scope.OrganizationName} | {scope.GradeName}";

    public async Task ShowAddSchedules()
    {
        SelectableScheduleOptions = AvailableOptions.Select(x => new Selectable
        {
            Id = x.Id,
            Name = ScheduleOptionName(x),
        }).ToObservable();
        await AddSchedulesModel.Show();
    }

    private static String ScheduleOptionName(CampaignScheduleOption option) => option.DistrictWide
        ? option.GradeName ?? String.Empty
        : $"{option.OrganizationName} | {option.GradeName}";

    public void AddSelectedSchedules()
    {
        var selected = SelectableScheduleOptions.Where(x => x.Selected == true)
            .Select(x => x.Id).ToHashSet();
        var options = AvailableOptions.Where(x => selected.Contains(x.Id)).ToList();
        foreach (var option in options) AddScope(option);
        AddSchedulesModel.Hide();
        RefreshOptions();
        RaisePropertyChanged(nameof(Overrides));
    }

    private CampaignScopeConfiguration? AddScope(CampaignScheduleOption option)
    {
        if (Configuration is null) return null;
        var root = Configuration.Scopes.Single(x => !x.ParentId.HasValue);

        var parent = option.DistrictWide
            ? root
            : Configuration.Scopes.FirstOrDefault(x =>
                x.OrganizationId == Configuration.OrganizationId && x.GradeId == option.GradeId) ?? root;
        var scope = new CampaignScopeConfiguration
        {
            ParentId = parent.Id,
            OrganizationId = option.OrganizationId,
            OrganizationName = option.OrganizationName,
            GradeId = option.GradeId,
            GradeName = option.GradeName,
            Name = option.DistrictWide ? option.GradeName : $"{option.OrganizationName} | {option.GradeName}",
            Start = parent.Start,
            LessonStart = parent.LessonStart,
            AssessmentStart = parent.AssessmentStart,
            Ordinal = Configuration.Scopes.Max(x => x.Ordinal) + 1,
            StageSchedules = parent.StageSchedules.Select(x => new CampaignStageScheduleConfiguration
            {
                StageId = x.StageId,
                Send = x.Send,
                Overridden = false,
            }).ToList(),
        };
        Configuration.Scopes.Add(scope);
        if (option.DistrictWide)
        {
            foreach (var school in Configuration.Scopes.Where(x =>
                         x.Id != scope.Id && x.OrganizationId != Configuration.OrganizationId &&
                         x.GradeId == scope.GradeId))
                school.ParentId = scope.Id;
        }
        RefreshInherited(scope);
        return scope;
    }

    public Task RemoveScope(Guid? id)
    {
        var scope = Configuration?.Scopes.FirstOrDefault(x => x.Id == id);
        if (Configuration is null || scope is null || !scope.ParentId.HasValue)
            return Task.CompletedTask;
        var parent = Parent(scope)!;
        foreach (var child in Configuration.Scopes.Where(x => x.ParentId == scope.Id).ToList())
        {
            child.ParentId = parent.Id;
            RefreshInherited(child);
        }
        Configuration.Scopes.Remove(scope);
        RefreshOptions();
        RaisePropertyChanged(nameof(Overrides));
        return Task.CompletedTask;
    }

    public Task SetStart(CampaignScopeConfiguration scope, DateOnly? start)
    {
        scope.Start = start;
        scope.StartOverridden = true;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    public Task SetLessonStart(CampaignScopeConfiguration scope, DateOnly? start)
    {
        scope.LessonStart = start;
        scope.LessonStartOverridden = true;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    public Task SetAssessmentStart(CampaignScopeConfiguration scope, DateOnly? start)
    {
        scope.AssessmentStart = start;
        scope.AssessmentStartOverridden = true;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    public Task SetStage(
        CampaignScopeConfiguration scope,
        CampaignStageScheduleConfiguration schedule,
        CampaignScheduleStage stage,
        DateOnly? date)
    {
        schedule.Send = date?.ToDateTime(stage.SendTime ?? TimeOnly.MinValue);
        schedule.Overridden = true;
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    public OverrideState StartState(CampaignScopeConfiguration scope) => State(scope, scope.StartOverridden);
    public OverrideState LessonStartState(CampaignScopeConfiguration scope) => State(scope, scope.LessonStartOverridden);
    public OverrideState AssessmentStartState(CampaignScopeConfiguration scope) => State(scope, scope.AssessmentStartOverridden);
    public OverrideState StageState(CampaignScopeConfiguration scope, CampaignStageScheduleConfiguration schedule) =>
        State(scope, schedule.Overridden);

    public Task SetStartState(CampaignScopeConfiguration scope, OverrideState state)
    {
        if (!scope.ParentId.HasValue) return Task.CompletedTask;
        scope.StartOverridden = state == OverrideState.Custom;
        if (!scope.StartOverridden) scope.Start = Parent(scope)?.Start;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    public Task SetLessonStartState(CampaignScopeConfiguration scope, OverrideState state)
    {
        if (!scope.ParentId.HasValue) return Task.CompletedTask;
        scope.LessonStartOverridden = state == OverrideState.Custom;
        if (!scope.LessonStartOverridden) scope.LessonStart = Parent(scope)?.LessonStart;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    public Task SetAssessmentStartState(CampaignScopeConfiguration scope, OverrideState state)
    {
        if (!scope.ParentId.HasValue) return Task.CompletedTask;
        scope.AssessmentStartOverridden = state == OverrideState.Custom;
        if (!scope.AssessmentStartOverridden) scope.AssessmentStart = Parent(scope)?.AssessmentStart;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    public Task SetStageState(
        CampaignScopeConfiguration scope,
        CampaignStageScheduleConfiguration schedule,
        CampaignScheduleStage stage,
        OverrideState state)
    {
        schedule.Overridden = state == OverrideState.Custom;
        if (!schedule.Overridden) schedule.Send = DerivedStage(scope, stage);
        RefreshChildren(scope);
        RaisePropertyChanged(nameof(Configuration));
        return Task.CompletedTask;
    }

    private OverrideState State(CampaignScopeConfiguration scope, Boolean overridden) =>
        !scope.ParentId.HasValue || overridden ? OverrideState.Custom : OverrideState.Default;

    private CampaignScopeConfiguration? Parent(CampaignScopeConfiguration scope) =>
        Configuration?.Scopes.FirstOrDefault(x => x.Id == scope.ParentId);

    private void RefreshInherited(CampaignScopeConfiguration scope)
    {
        var parent = Parent(scope);
        if (parent is null) return;
        if (!scope.StartOverridden) scope.Start = parent.Start;
        if (!scope.LessonStartOverridden) scope.LessonStart = parent.LessonStart;
        if (!scope.AssessmentStartOverridden) scope.AssessmentStart = parent.AssessmentStart;
        RefreshDerivedStages(scope);
        RefreshChildren(scope);
    }

    private void RefreshChildren(CampaignScopeConfiguration scope)
    {
        if (Configuration is null) return;
        foreach (var child in Configuration.Scopes.Where(x => x.ParentId == scope.Id)) RefreshInherited(child);
    }

    private void RefreshDerivedStages(CampaignScopeConfiguration scope)
    {
        if (Configuration is null) return;
        foreach (var stage in Configuration.Stages)
        {
            var schedule = scope.StageSchedules.Single(x => x.StageId == stage.StageId);
            if (!schedule.Overridden) schedule.Send = DerivedStage(scope, stage);
        }
    }

    private DateTime? DerivedStage(CampaignScopeConfiguration scope, CampaignScheduleStage stage)
    {
        var parent = Parent(scope);
        if (parent is not null)
        {
            var send = parent.StageSchedules.Single(x => x.StageId == stage.StageId).Send;
            var parentAnchor = CampaignScheduleCalculator.GetAnchor(stage.Anchor,
                parent.Start, parent.LessonStart, parent.AssessmentStart);
            var anchor = CampaignScheduleCalculator.GetAnchor(stage.Anchor,
                scope.Start, scope.LessonStart, scope.AssessmentStart);
            if (!send.HasValue || !parentAnchor.HasValue || !anchor.HasValue) return send;
            return send.Value.AddDays(anchor.Value.DayNumber - parentAnchor.Value.DayNumber);
        }

        return CampaignScheduleCalculator.CalculateDate(
                stage.Anchor, stage.Offset, stage.WeekendAdjustment,
                scope.Start, scope.LessonStart, scope.AssessmentStart)
            ?.ToDateTime(stage.SendTime ?? TimeOnly.MinValue);
    }

    public async Task Save()
    {
        if (Configuration is null || !IsValid(Configuration)) return;
        var response = await WithWaiting("Saving campaign schedule...", () =>
            activationService.SaveSchedule(new(Configuration)));
        if (response.Ok) await Fetch(false);
    }
}