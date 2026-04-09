namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IAuthService
{
    Task<Response<AuthResult?>> CheckCredentials(Request<Credentials> request);
    Task<Response<AuthResult?>> CheckAccessCode(Request<AccessCode> request);
    Task<Response> ResetPassword(Request<AccessCode> request);
    Task<Response> ChangePassword(Request<PasswordChange> request);
    Task<Response> SignOut(Request request);
}