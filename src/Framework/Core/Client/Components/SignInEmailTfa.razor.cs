namespace Crudspa.Framework.Core.Client.Components;

public partial class SignInEmailTfa : IDisposable
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

    public SignInEmailTfaModel? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Model = new(ScrollService, AuthService, CookieService, Navigator, SessionState, Portal.AllowSignIn);
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

public class SignInEmailTfaModel(
    IScrollService scrollService,
    IAuthService authService,
    ICookieService cookieService,
    INavigator navigator,
    ISessionState sessionState,
    Boolean? allowSignIn)
    : ModalModel(scrollService)
{
    public enum States
    {
        EnterCredentials,
        EnterCode,
        ResetPassword,
        ChangePassword,
        SignOut,
    }

    public Boolean? AllowSignIn
    {
        get;
        set => SetProperty(ref field, value);
    } = allowSignIn;

    public States State
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Username
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Password
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean RememberMe
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Boolean KeepMeSignedIn
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public String? AccessCode
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? NewPassword
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ConfirmPassword
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean ResetOnEntry
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override async Task Hide()
    {
        await base.Hide();
        ClearFields();
        ResetOnEntry = false;
    }

    public void ClearFields()
    {
        Password = AccessCode = NewPassword = ConfirmPassword = String.Empty;
        if (!RememberMe) Username = String.Empty;
    }

    public async Task ShowIfNeeded()
    {
        if (!sessionState.IsSignedIn)
            await ShowEnterCredentials();
    }

    public async Task ShowEnterCredentials()
    {
        State = States.EnterCredentials;
        Alerts.Clear();

        Username = await cookieService.Get(Constants.CookieKeys.Username, String.Empty);
        Password = String.Empty;

        await Show();
    }

    public async Task ShowEnterCode()
    {
        State = States.EnterCode;
        Alerts.Clear();
        await Show();
    }

    public async Task ShowResetPassword()
    {
        State = States.ResetPassword;
        Alerts.Clear();
        await Show();
    }

    public async Task ShowChangePassword()
    {
        State = States.ChangePassword;
        Alerts.Clear();
        await Show();
    }

    public void ShowAccountSettings()
    {
        navigator.GoTo("/settings?pane=account");
    }

    public async Task ShowSignOut()
    {
        State = States.SignOut;
        Alerts.Clear();
        await Show();
    }

    public async Task CheckCredentials()
    {
        TrimStrings();

        var request = new Request<Credentials>(new()
        {
            Username = Username,
            Password = Password,
        });

        var response = await WithWaiting("Verifying...", () => authService.CheckCredentials(request));

        if (!response.Ok)
            return;

        switch (response.Value.Result)
        {
            case AuthResult.Results.SessionNotStarted:
                Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Session could not be started. Please refresh.", Dismissible = false });
                break;
            case AuthResult.Results.PasswordNotSet:
                await ShowEnterCode();
                Alerts.Add(new() { Type = Alert.AlertType.Warning, Message = "An access code was sent to your email address.", Dismissible = false });
                break;
            case AuthResult.Results.CredentialsInvalid:
            case AuthResult.Results.CredentialsIncorrect:
                Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Sign in failed.", Dismissible = false });
                break;
            case AuthResult.Results.CredentialsCorrect:
                await ShowEnterCode();
                Alerts.Add(new() { Type = Alert.AlertType.Success, Message = "An access code was sent to your email address.", Dismissible = false });
                break;
            case AuthResult.Results.AccessCodeAccepted:
            case AuthResult.Results.AccessCodeDenied:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task EnterCode()
    {
        TrimStrings();

        var response = await WithWaiting("Verifying...", () => authService.CheckAccessCode(new(new()
        {
            Username = Username,
            Code = AccessCode,
        })));

        if (!response.Ok || response.Value.Result != AuthResult.Results.AccessCodeAccepted)
        {
            Alerts.Add(new() { Type = Alert.AlertType.Error, Message = "Access code denied." });
            return;
        }

        Waiting = true;
        WaitingOn = "Initializing...";

        await sessionState.Initialize(response.Value.SessionId, KeepMeSignedIn);

        Waiting = false;

        if (RememberMe)
            await cookieService.Set(Constants.CookieKeys.Username, Username!, DateTimeOffset.Now.AddDays(90));
        else
            await cookieService.Set(Constants.CookieKeys.Username, String.Empty, expires: null);

        if (sessionState.Session.User?.ResetPassword == true)
        {
            ResetOnEntry = true;
            await ShowChangePassword();
            Alerts.Add(new() { Type = Alert.AlertType.Tip, Message = "Please establish a new password." });
        }
        else
        {
            ResetOnEntry = false;
            await Hide();
        }
    }

    public async Task ResetPassword()
    {
        TrimStrings();

        var response = await WithWaiting("Requesting...", () => authService.ResetPassword(new(new() { Username = Username })));

        if (response.Ok)
        {
            await ShowEnterCode();
            Alerts.Add(new() { Type = Alert.AlertType.Success, Message = "Access code sent." });
        }
    }

    public async Task ChangePassword()
    {
        var response = await WithWaiting("Setting...", () => authService.ChangePassword(new(new()
        {
            NewPassword = NewPassword,
            Confirm = ConfirmPassword,
        })));

        if (!response.Ok)
            return;

        WaitingOn = "Password changed!";
        Waiting = true;

        await Task.Delay(500);

        Waiting = false;
        await Hide();
    }

    public async Task SignOut()
    {
        WaitingOn = "Signing out...";
        Waiting = true;

        await authService.SignOut(new());

        await cookieService.Set(Constants.CookieKeys.SessionId, String.Empty, DateTimeOffset.Now.AddDays(-1));

        navigator.Bounce();
    }

    private void TrimStrings()
    {
        Username = Username?.Trim();
        AccessCode = AccessCode?.Trim();
    }
}