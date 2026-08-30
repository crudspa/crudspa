namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class AuthResult : Observable
{
    public enum Results
    {
        SessionNotStarted,
        CredentialsIncorrect,
        PasswordRequired,
        CredentialsCorrect,
        External,
        AccessCodeAccepted,
        AccessCodeDenied,
    }

    public Results Result
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SessionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RedirectUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean ResetPassword
    {
        get;
        set => SetProperty(ref field, value);
    }
}