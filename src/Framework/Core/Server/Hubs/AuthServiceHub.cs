namespace Crudspa.Framework.Core.Server.Hubs;

public partial class CoreHub
{
    public async Task<Response<AuthResult?>> AuthCheckCredentials(Request<Credentials> request) =>
        await HubWrappers.RequireSession(request, async session =>
        {
            var response = await AuthService.CheckCredentials(request);

            if (!response.Ok || response.Value.Result != AuthResult.Results.CredentialsCorrect)
                await AccessDeniedService.Add(new()
                {
                    SessionId = session.Id,
                    EventType = Constants.AccessDeniedEventTypes.AuthFailed,
                    Method = "Crudspa.Framework.Core.Server.Hubs.CoreHub.AuthCheckCredentials",
                });

            return response;
        });

    public async Task<Response<AuthResult?>> AuthCheckAccessCode(Request<AccessCode> request) =>
        await HubWrappers.RequireSession(request, async session =>
        {
            request.Value.PortalId = ServerConfigService.Fetch().PortalId;
            var response = await AuthService.CheckAccessCode(request);

            if (!response.Ok || response.Value.Result != AuthResult.Results.AccessCodeAccepted)
                await AccessDeniedService.Add(new()
                {
                    SessionId = session.Id,
                    EventType = Constants.AccessDeniedEventTypes.AccessCodeFailed,
                    Method = "Crudspa.Framework.Core.Server.Hubs.CoreHub.AuthCheckAccessCode",
                });

            return response;
        });

    public async Task<Response> AuthResetPassword(Request<AccessCode> request) =>
        await HubWrappers.RequireSession(request, async session =>
        {
            request.Value.PortalId = ServerConfigService.Fetch().PortalId;
            return await AuthService.ResetPassword(request);
        });

    public async Task<Response> AuthChangePassword(Request<PasswordChange> request) =>
        await HubWrappers.RequireSession(request, async session =>
            await AuthService.ChangePassword(request));

    public async Task<Response> AuthSignOut(Request request) =>
        await HubWrappers.RequireSession(request, async session =>
            await AuthService.SignOut(request));
}