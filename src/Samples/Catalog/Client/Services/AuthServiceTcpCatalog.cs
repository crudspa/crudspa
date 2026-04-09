namespace Crudspa.Samples.Catalog.Client.Services;

public class AuthServiceTcpCatalog(IProxyWrappers proxyWrappers) : IAuthService
{
    public async Task<Response<AuthResult?>> CheckCredentials(Request<Credentials> request) =>
        await proxyWrappers.Send<AuthResult?>("AuthCheckCredentials", request);

    public async Task<Response<AuthResult?>> CheckAccessCode(Request<AccessCode> request) =>
        await proxyWrappers.Send<AuthResult?>("AuthCheckAccessCode", request);

    public async Task<Response> ResetPassword(Request<AccessCode> request) =>
        await proxyWrappers.Send("AuthResetPassword", request);

    public async Task<Response> ChangePassword(Request<PasswordChange> request) =>
        await proxyWrappers.Send("AuthChangePassword", request);

    public async Task<Response> SignOut(Request request) =>
        await proxyWrappers.Send("AuthSignOut", request);
}