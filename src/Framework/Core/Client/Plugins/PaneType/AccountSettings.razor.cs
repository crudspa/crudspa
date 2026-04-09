namespace Crudspa.Framework.Core.Client.Plugins.PaneType;

public partial class AccountSettings : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IAccountSettingsService AccountSettingsService { get; set; } = null!;

    public AccountSettingsModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, AccountSettingsService, SessionState);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        await Model.Refresh();
    }
}

public class AccountSettingsModel : EditModel<User>
{
    private readonly IAccountSettingsService _accountSettingsService;
    private readonly ISessionState _sessionState;

    public AccountSettingsModel(IEventBus eventBus,
        IAccountSettingsService accountSettingsService,
        ISessionState sessionState) : base(false)
    {
        _accountSettingsService = accountSettingsService;
        _sessionState = sessionState;

        eventBus.Subscribe(this);
    }

    public async Task Refresh()
    {
        ReadOnly = true;

        var response = await WithWaiting("Fetching...", () => _accountSettingsService.Fetch(new()));

        if (response.Ok)
            Entity = response.Value;
    }

    public async Task Save()
    {
        var response = await WithWaiting("Saving...", () => _accountSettingsService.Save(new(Entity!)));

        if (response.Ok)
        {
            ReadOnly = true;
            await _sessionState.Refresh(_sessionState.Session.Id);
        }
    }
}