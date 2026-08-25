namespace Crudspa.Content.Messaging.Client.Plugins.PaneType;

public partial class SmsPreferenceEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public ISmsPreferenceService SmsPreferenceService { get; set; } = null!;

    public SmsPreferenceEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var portalId = Path!.Id("portal") ?? SessionState.Session.PortalId;

        Model = new(Path, Id, IsNew, portalId, EventBus, Navigator, SmsPreferenceService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class SmsPreferenceEditModel : EditModel<SmsPreference>,
    IHandle<SmsPreferenceSaved>, IHandle<SmsPreferenceRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _portalId;
    private readonly INavigator _navigator;
    private readonly ISmsPreferenceService _smsPreferenceService;

    public SmsPreferenceEditModel(String? path, Guid? id, Boolean isNew, Guid? portalId,
        IEventBus eventBus,
        INavigator navigator,
        ISmsPreferenceService smsPreferenceService) : base(isNew)
    {
        _path = path;
        _id = id;
        _portalId = portalId;
        _navigator = navigator;
        _smsPreferenceService = smsPreferenceService;

        eventBus.Subscribe(this);
    }

    public async Task Handle(SmsPreferenceSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SmsPreferenceRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            var smsPreference = new SmsPreference
            {
                PortalId = _portalId,
                Number = String.Empty,
                Source = SmsPreference.Sources.Staff,
                StatusChanged = DateTimeOffset.Now,
            };

            SetSmsPreference(smsPreference);
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () => _smsPreferenceService.Fetch(new(new() { Id = _id })));

            if (response.Ok)
                SetSmsPreference(response.Value);
        }
    }

    public async Task Save()
    {
        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _smsPreferenceService.Add(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/sms-preference-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _smsPreferenceService.Save(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private void SetSmsPreference(SmsPreference smsPreference)
    {
        Entity = smsPreference;
        _navigator.UpdateTitle(_path, Entity.Number);
    }
}