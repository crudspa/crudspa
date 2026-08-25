namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;
using SurveyAdded = Crudspa.Content.Design.Shared.Contracts.Events.SurveyAdded;
using SurveySaved = Crudspa.Content.Design.Shared.Contracts.Events.SurveySaved;
using SurveyRemoved = Crudspa.Content.Design.Shared.Contracts.Events.SurveyRemoved;

public partial class SurveyLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISurveyLicenseService SurveyLicenseService { get; set; } = null!;

    public SurveyLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, SurveyLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class SurveyLicenseManyForLicenseModel : ManyModel<SurveyLicenseModel>,
    IHandle<SurveyLicenseAdded>, IHandle<SurveyLicenseSaved>, IHandle<SurveyLicenseRemoved>,
    IHandle<SurveyAdded>, IHandle<SurveySaved>, IHandle<SurveyRemoved>
{
    private readonly ISurveyLicenseService _surveyLicenseService;
    private readonly Guid? _licenseId;

    public SurveyLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, ISurveyLicenseService surveyLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _surveyLicenseService = surveyLicenseService;

        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SurveyLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(SurveyLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(SurveyLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(SurveyAdded payload) => await FetchSurveyNames();

    public async Task Handle(SurveySaved payload) => await FetchSurveyNames();

    public async Task Handle(SurveyRemoved payload) => await FetchSurveyNames();

    public ObservableCollection<Named> SurveyNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchSurveyNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _surveyLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new SurveyLicenseModel(x)));
    }

    public override async Task Create()
    {
        var surveyLicense = new SurveyLicense
        {
            Id = Guid.NewGuid(),
            SurveyId = SurveyNames.FirstOrDefault()?.Id,
            LicenseId = _licenseId,
        };

        var form = await CreateForm(new(surveyLicense));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<SurveyLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _surveyLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new SurveyLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<SurveyLicenseModel?>> Add(FormModel<SurveyLicenseModel> form)
    {
        var response = await _surveyLicenseService.Add(new(form.Entity.SurveyLicense));

        return response.Ok
            ? new(new SurveyLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<SurveyLicenseModel> form)
    {
        var surveyLicense = form.Entity.SurveyLicense;

        return await _surveyLicenseService.Save(new(surveyLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _surveyLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchSurveyNames()
    {
        var response = await WithAlerts(() => _surveyLicenseService.FetchSurveyNames(new()), false);
        if (response.Ok) SurveyNames = response.Value.ToObservable();
    }
}

public class SurveyLicenseModel : Observable, IDisposable, INamed
{
    private void HandleSurveyLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(SurveyLicense));

    private SurveyLicense _surveyLicense;

    public String? Name => SurveyLicense.SurveyTitle;

    public SurveyLicenseModel(SurveyLicense surveyLicense)
    {
        _surveyLicense = surveyLicense;
        _surveyLicense.PropertyChanged += HandleSurveyLicenseChanged;
    }

    public void Dispose()
    {
        _surveyLicense.PropertyChanged -= HandleSurveyLicenseChanged;
    }

    public Guid? Id
    {
        get => _surveyLicense.Id;
        set => _surveyLicense.Id = value;
    }

    public SurveyLicense SurveyLicense
    {
        get => _surveyLicense;
        set => SetProperty(ref _surveyLicense, value);
    }
}