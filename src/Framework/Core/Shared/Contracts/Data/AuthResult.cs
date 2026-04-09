namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class AuthResult : Observable
{
    public enum Results
    {
        SessionNotStarted,
        PasswordNotSet,
        CredentialsInvalid,
        CredentialsIncorrect,
        CredentialsCorrect,
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
}