namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

public partial class SchoolYearMany : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISchoolYearService SchoolYearService { get; set; } = null!;

    public SchoolYearManyModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SchoolYearService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SchoolYearManyModel : ManyModel<SchoolYearModel>,
    IHandle<SchoolYearAdded>, IHandle<SchoolYearSaved>, IHandle<SchoolYearRemoved>
{
    private readonly ISchoolYearService _schoolYearService;

    public SchoolYearManyModel(IEventBus eventBus, IScrollService scrollService, ISchoolYearService schoolYearService)
        : base(scrollService, false)
    {
        _schoolYearService = schoolYearService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SchoolYearAdded payload) => await Replace(payload.Id);

    public async Task Handle(SchoolYearSaved payload) => await Replace(payload.Id);

    public async Task Handle(SchoolYearRemoved payload) => await Rid(payload.Id, null);

    public async Task Initialize()
    {
        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request(Guid.Empty);
        var response = await WithWaiting("Fetching...", () => _schoolYearService.FetchAll(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new SchoolYearModel(x)));
    }

    public override async Task Create()
    {
        var schoolYear = new SchoolYear
        {
            Id = Guid.NewGuid(),
            Name = "New School Year",
            Starts = DateOnly.FromDateTime(DateTime.Now),
            Ends = DateOnly.FromDateTime(DateTime.Now),
        };

        var form = await CreateForm(new(schoolYear));
    }

    public override async Task<Response<SchoolYearModel?>> Fetch(Guid? id)
    {
        var response = await _schoolYearService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new SchoolYearModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<SchoolYearModel?>> Add(FormModel<SchoolYearModel> form)
    {
        var response = await _schoolYearService.Add(new(form.Entity.SchoolYear));

        return response.Ok
            ? new(new SchoolYearModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<SchoolYearModel> form)
    {
        var schoolYear = form.Entity.SchoolYear;

        return await _schoolYearService.Save(new(schoolYear));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _schoolYearService.Remove(new(new()
        {
            Id = id,
        }));
    }

    public override String? OrderBy(FormModel<SchoolYearModel> form)
    {
        var schoolYear = form.Entity.SchoolYear;

        if (!schoolYear.Starts.HasValue || !schoolYear.Ends.HasValue)
            return schoolYear.Name;

        return schoolYear.Starts.Value.ToString("yyyy-MM-dd") + schoolYear.Ends.Value.ToString("yyyy-MM-dd");
    }
}

public class SchoolYearModel : Observable, IDisposable, INamed
{
    private void HandleSchoolYearChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(SchoolYear));

    private SchoolYear _schoolYear;

    public String? Name => SchoolYear.Name;

    public SchoolYearModel(SchoolYear schoolYear)
    {
        _schoolYear = schoolYear;
        _schoolYear.PropertyChanged += HandleSchoolYearChanged;
    }

    public void Dispose()
    {
        _schoolYear.PropertyChanged -= HandleSchoolYearChanged;
    }

    public Guid? Id
    {
        get => _schoolYear.Id;
        set => _schoolYear.Id = value;
    }

    public SchoolYear SchoolYear
    {
        get => _schoolYear;
        set => SetProperty(ref _schoolYear, value);
    }
}