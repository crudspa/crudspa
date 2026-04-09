namespace Crudspa.Samples.Catalog.Client.Components;

public partial class SignInCatalogName : IDisposable
{
    private void HandleChanged(Object? sender, EventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public Portal Portal { get; set; } = null!;
    [Parameter] public RenderFragment? LeftPanel { get; set; }
    [Parameter] public RenderFragment? MenuItems { get; set; }

    [Inject] public ICookieService CookieService { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public ISessionState SessionState { get; set; } = null!;
    [Inject] public IAuthService AuthService { get; set; } = null!;

    public SignInCatalogNameModel? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = new(ScrollService, AuthService, CookieService, Navigator, SessionState);
        Model.PropertyChanged += HandleChanged;
        SessionState.SessionInitialized += HandleChanged;
        SessionState.SessionRefreshed += HandleChanged;

        if (Portal.RequireSignIn == true)
            await Model.ShowIfNeeded();
    }

    public void Dispose()
    {
        if (Model is null)
            return;

        Model.PropertyChanged -= HandleChanged;
        SessionState.SessionInitialized -= HandleChanged;
        SessionState.SessionRefreshed -= HandleChanged;
        Model.Dispose();
    }
}

public class SignInCatalogNameModel(
    IScrollService scrollService,
    IAuthService authService,
    ICookieService cookieService,
    INavigator navigator,
    ISessionState sessionState)
    : ModalModel(scrollService)
{
    public enum States
    {
        SignIn,
        SignOut,
    }

    public States State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override async Task Hide()
    {
        await base.Hide();
        Name = String.Empty;
    }

    public async Task ShowIfNeeded()
    {
        if (!sessionState.IsSignedIn)
            await ShowSignIn();
    }

    public async Task ShowSignIn()
    {
        State = States.SignIn;
        Alerts.Clear();
        Alerts.Add(new() { Type = Alert.AlertType.Tip, Message = "Enter your name to simulate signing in.", Dismissible = false });
        await Show();
    }

    public async Task ShowSignOut()
    {
        State = States.SignOut;
        Alerts.Clear();
        await Show();
    }

    public async Task SignIn()
    {
        Name = Name?.Trim();
        Alerts.RemoveWhere(x => x.Dismissible);

        if (Name.HasNothing())
        {
            Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Name is required." });
            return;
        }

        if (Name!.Length > 75)
        {
            Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Name cannot be longer than 75 characters." });
            return;
        }

        var response = await WithWaiting("Signing in...", () => authService.CheckCredentials(new(new()
        {
            Username = Name,
        })));

        if (!response.Ok)
            return;

        switch (response.Value.Result)
        {
            case AuthResult.Results.CredentialsCorrect when response.Value.SessionId is not null:
                WaitingOn = "Initializing...";
                Waiting = true;
                await sessionState.Initialize(response.Value.SessionId, true);
                Waiting = false;
                await Hide();
                break;
            case AuthResult.Results.SessionNotStarted:
                Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Session could not be started. Please refresh." });
                break;
            case AuthResult.Results.CredentialsInvalid:
            case AuthResult.Results.CredentialsIncorrect:
            case AuthResult.Results.PasswordNotSet:
            case AuthResult.Results.AccessCodeAccepted:
            case AuthResult.Results.AccessCodeDenied:
            case AuthResult.Results.CredentialsCorrect:
            default:
                Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Sign in failed." });
                break;
        }
    }

    public async Task SignOut()
    {
        WaitingOn = "Signing out...";
        Waiting = true;

        await authService.SignOut(new());
        await cookieService.Set(Constants.CookieKeys.SessionId, String.Empty, DateTimeOffset.Now.AddDays(-1));

        navigator.Bounce();
    }
}