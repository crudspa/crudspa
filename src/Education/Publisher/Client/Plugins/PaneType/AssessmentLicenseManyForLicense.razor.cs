namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;
using AssessmentAdded = Crudspa.Education.Publisher.Shared.Contracts.Events.AssessmentAdded;
using AssessmentSaved = Crudspa.Education.Publisher.Shared.Contracts.Events.AssessmentSaved;
using AssessmentRemoved = Crudspa.Education.Publisher.Shared.Contracts.Events.AssessmentRemoved;

public partial class AssessmentLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IAssessmentLicenseService AssessmentLicenseService { get; set; } = null!;

    public AssessmentLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, AssessmentLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class AssessmentLicenseManyForLicenseModel : ManyModel<AssessmentLicenseModel>,
    IHandle<AssessmentLicenseAdded>, IHandle<AssessmentLicenseSaved>, IHandle<AssessmentLicenseRemoved>,
    IHandle<AssessmentAdded>, IHandle<AssessmentSaved>, IHandle<AssessmentRemoved>
{
    private readonly IAssessmentLicenseService _assessmentLicenseService;
    private readonly Guid? _licenseId;

    public AssessmentLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, IAssessmentLicenseService assessmentLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _assessmentLicenseService = assessmentLicenseService;

        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(AssessmentLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(AssessmentLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(AssessmentLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(AssessmentAdded payload) => await FetchAssessmentNames();

    public async Task Handle(AssessmentSaved payload) => await FetchAssessmentNames();

    public async Task Handle(AssessmentRemoved payload) => await FetchAssessmentNames();

    public ObservableCollection<Named> AssessmentNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchAssessmentNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _assessmentLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new AssessmentLicenseModel(x)));
    }

    public override async Task Create()
    {
        var assessmentLicense = new AssessmentLicense
        {
            Id = Guid.NewGuid(),
            AssessmentId = AssessmentNames.MinBy(x => x.Name)?.Id,
            LicenseId = _licenseId,
        };

        var form = await CreateForm(new(assessmentLicense));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<AssessmentLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _assessmentLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new AssessmentLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<AssessmentLicenseModel?>> Add(FormModel<AssessmentLicenseModel> form)
    {
        var response = await _assessmentLicenseService.Add(new(form.Entity.AssessmentLicense));

        return response.Ok
            ? new(new AssessmentLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<AssessmentLicenseModel> form)
    {
        var assessmentLicense = form.Entity.AssessmentLicense;

        return await _assessmentLicenseService.Save(new(assessmentLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _assessmentLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchAssessmentNames()
    {
        var response = await WithAlerts(() => _assessmentLicenseService.FetchAssessmentNames(new()), false);
        if (response.Ok) AssessmentNames = response.Value.ToObservable();
    }
}

public class AssessmentLicenseModel : Observable, IDisposable, INamed
{
    private void HandleAssessmentLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(AssessmentLicense));

    private AssessmentLicense _assessmentLicense;

    public String? Name => AssessmentLicense.AssessmentName;

    public AssessmentLicenseModel(AssessmentLicense assessmentLicense)
    {
        _assessmentLicense = assessmentLicense;
        _assessmentLicense.PropertyChanged += HandleAssessmentLicenseChanged;
    }

    public void Dispose()
    {
        _assessmentLicense.PropertyChanged -= HandleAssessmentLicenseChanged;
    }

    public Guid? Id
    {
        get => _assessmentLicense.Id;
        set => _assessmentLicense.Id = value;
    }

    public AssessmentLicense AssessmentLicense
    {
        get => _assessmentLicense;
        set => SetProperty(ref _assessmentLicense, value);
    }
}