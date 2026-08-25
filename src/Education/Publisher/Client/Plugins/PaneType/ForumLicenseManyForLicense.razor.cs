namespace Crudspa.Education.Publisher.Client.Plugins.PaneType;

using License = Shared.Contracts.Data.License;
using ForumAdded = Crudspa.Content.Design.Shared.Contracts.Events.ForumAdded;
using ForumSaved = Crudspa.Content.Design.Shared.Contracts.Events.ForumSaved;
using ForumRemoved = Crudspa.Content.Design.Shared.Contracts.Events.ForumRemoved;

public partial class ForumLicenseManyForLicense : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IForumLicenseService ForumLicenseService { get; set; } = null!;

    public ForumLicenseManyForLicenseModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, ForumLicenseService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ForumLicenseManyForLicenseModel : ManyModel<ForumLicenseModel>,
    IHandle<ForumLicenseAdded>, IHandle<ForumLicenseSaved>, IHandle<ForumLicenseRemoved>,
    IHandle<ForumAdded>, IHandle<ForumSaved>, IHandle<ForumRemoved>, IHandle<ForumsReordered>
{
    private readonly IForumLicenseService _forumLicenseService;
    private readonly Guid? _licenseId;

    public ForumLicenseManyForLicenseModel(IEventBus eventBus, IScrollService scrollService, IForumLicenseService forumLicenseService, Guid? licenseId)
        : base(scrollService)
    {
        _forumLicenseService = forumLicenseService;

        _licenseId = licenseId;

        eventBus.Subscribe(this);
    }

    public async Task Handle(ForumLicenseAdded payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(ForumLicenseSaved payload) => await Replace(payload.Id, payload.LicenseId);

    public async Task Handle(ForumLicenseRemoved payload) => await Rid(payload.Id, payload.LicenseId);

    public async Task Handle(ForumAdded payload) => await FetchForumNames();

    public async Task Handle(ForumSaved payload) => await FetchForumNames();

    public async Task Handle(ForumRemoved payload) => await FetchForumNames();

    public async Task Handle(ForumsReordered payload) => await FetchForumNames();

    public ObservableCollection<Orderable> ForumNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchForumNames());

        await Refresh();
    }

    public override async Task Refresh(Boolean resetAlerts = true)
    {
        var request = new Request<License>(new() { Id = _licenseId });
        var response = await WithWaiting("Fetching...", () => _forumLicenseService.FetchForLicense(request), resetAlerts);

        if (response.Ok)
            SetForms(response.Value.Select(x => new ForumLicenseModel(x)));
    }

    public override async Task Create()
    {
        var forumLicense = new ForumLicense
        {
            Id = Guid.NewGuid(),
            ForumId = ForumNames.MinBy(x => x.Ordinal)?.Id,
            LicenseId = _licenseId,
        };

        var form = await CreateForm(new(forumLicense));
    }

    public override Boolean InScope(Guid? scopeId)
    {
        return scopeId is null || scopeId.Equals(_licenseId);
    }

    public override async Task<Response<ForumLicenseModel?>> Fetch(Guid? id)
    {
        var response = await _forumLicenseService.Fetch(new(new() { Id = id }));

        return response.Ok
            ? new(new ForumLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response<ForumLicenseModel?>> Add(FormModel<ForumLicenseModel> form)
    {
        var response = await _forumLicenseService.Add(new(form.Entity.ForumLicense));

        return response.Ok
            ? new(new ForumLicenseModel(response.Value))
            : new() { Errors = response.Errors };
    }

    public override async Task<Response> Save(FormModel<ForumLicenseModel> form)
    {
        var forumLicense = form.Entity.ForumLicense;

        return await _forumLicenseService.Save(new(forumLicense));
    }

    public override async Task<Response> Remove(Guid? id)
    {
        return await _forumLicenseService.Remove(new(new()
        {
            Id = id,
            LicenseId = _licenseId,
        }));
    }

    public async Task FetchForumNames()
    {
        var response = await WithAlerts(() => _forumLicenseService.FetchForumNames(new()), false);
        if (response.Ok) ForumNames = response.Value.ToObservable();
    }
}

public class ForumLicenseModel : Observable, IDisposable, INamed
{
    private void HandleForumLicenseChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ForumLicense));

    private ForumLicense _forumLicense;

    public String? Name => ForumLicense.ForumTitle;

    public ForumLicenseModel(ForumLicense forumLicense)
    {
        _forumLicense = forumLicense;
        _forumLicense.PropertyChanged += HandleForumLicenseChanged;
    }

    public void Dispose()
    {
        _forumLicense.PropertyChanged -= HandleForumLicenseChanged;
    }

    public Guid? Id
    {
        get => _forumLicense.Id;
        set => _forumLicense.Id = value;
    }

    public ForumLicense ForumLicense
    {
        get => _forumLicense;
        set => SetProperty(ref _forumLicense, value);
    }
}