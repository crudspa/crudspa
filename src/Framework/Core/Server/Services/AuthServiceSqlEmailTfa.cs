namespace Crudspa.Framework.Core.Server.Services;

public class AuthServiceSqlEmailTfa(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    ISessionService sessionService,
    ICryptographyService cryptographyService,
    ISessionFetcher sessionFetcher,
    INativeAuthPolicy authPolicy,
    IEnumerable<ISessionAuthService> sessionAuthServices,
    IConfiguration configuration)
    : IAuthService
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public async Task<Response<AuthResult?>> CheckCredentials(Request<Credentials> request)
    {
        return await wrappers.Try<AuthResult?>(request, async response =>
        {
            var credentials = request.Value;

            if (credentials.Username.HasNothing() || credentials.Username!.Length > 75)
                return new() { Result = AuthResult.Results.CredentialsIncorrect };

            var user = await UserSelectByUsername.Execute(Connection, PortalId, credentials.Username!);

            if (user is null)
                return new()
                {
                    Result = credentials.Password.HasNothing()
                        ? AuthResult.Results.PasswordRequired
                        : AuthResult.Results.CredentialsIncorrect,
                };

            var method = await authPolicy.Resolve(user.Id!.Value);

            if (method is null)
            {
                var route = await authPolicy.ResolveExternal(user.Id.Value);
                if (route is not null)
                    return new()
                    {
                        Result = AuthResult.Results.External,
                        RedirectUrl = BuildRedirectUrl(route),
                    };
            }

            if (method == NativeAuthMethod.PasswordEmailCode)
            {
                if (credentials.Password.HasNothing())
                    return new() { Result = AuthResult.Results.PasswordRequired };

                if (user.PasswordHash is null || user.PasswordSalt is null)
                    return new() { Result = AuthResult.Results.CredentialsIncorrect };

                var hash = cryptographyService.ComputeHash(credentials.Password!, user.PasswordSalt);

                if (!hash.SequenceEqual(user.PasswordHash))
                    return new() { Result = AuthResult.Results.CredentialsIncorrect };
            }
            else if (method != NativeAuthMethod.EmailCode)
                return new() { Result = AuthResult.Results.CredentialsIncorrect };

            var accessCodeResponse = await accessCodeService.Generate(new(request.SessionId, user));

            if (!accessCodeResponse.Ok)
            {
                response.AddErrors(accessCodeResponse.Errors);
                return new() { Result = AuthResult.Results.CredentialsIncorrect };
            }

            return new() { Result = AuthResult.Results.CredentialsCorrect };
        });
    }

    private String BuildRedirectUrl(ExternalAuthRoute route)
    {
        var authUrl = configuration["Crudspa.Framework.Core.Server.AuthUrl"];
        if (!Uri.TryCreate(authUrl, UriKind.Absolute, out var authority)
            || authority.Scheme != Uri.UriSchemeHttps
            || authority.UserInfo.HasSomething()
            || authority.Query.HasSomething()
            || authority.Fragment.HasSomething())
            throw new InvalidOperationException("Auth URL must be an absolute HTTPS URL without user information, query, or fragment.");

        var path = $"auth/{Uri.EscapeDataString(route.Provider)}/start";
        var query = QueryString.Create(new KeyValuePair<String, String?>[]
        {
            new("audience", route.Audience),
            new("tenant", route.Tenant),
            new("returnPath", "/"),
        });

        return new Uri(authority, path + query).ToString();
    }

    public async Task<Response<AuthResult?>> CheckAccessCode(Request<AccessCode> request)
    {
        return await wrappers.Try<AuthResult?>(request, async response =>
        {
            var accessCode = request.Value;

            if (accessCode.Username.HasNothing())
                return new() { Result = AuthResult.Results.CredentialsIncorrect };

            var session = new Session { Id = request.SessionId };
            var sessionValidResponse = await sessionService.IsValidForSignIn(new(session));

            if (!sessionValidResponse.Ok)
            {
                var sessionCreateResponse = await sessionService.FetchOrCreate(new(new()));

                if (!sessionCreateResponse.Ok)
                    return new() { Result = AuthResult.Results.SessionNotStarted };

                session.Id = sessionCreateResponse.Value.Id;
            }

            var user = await UserSelectByUsername.Execute(Connection, accessCode.PortalId, accessCode.Username!);

            if (user is null)
                return new() { Result = AuthResult.Results.CredentialsIncorrect };

            var method = await authPolicy.Resolve(user.Id!.Value);

            if (method is not NativeAuthMethod.PasswordEmailCode and not NativeAuthMethod.EmailCode)
                return new() { Result = AuthResult.Results.AccessCodeDenied };

            var success = await AccessCodeUpdate.Execute(Connection, user.Id, accessCode);

            if (!success)
                return new() { Result = AuthResult.Results.AccessCodeDenied };

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SessionUpdateUser.Execute(connection, transaction, session.Id, user.Id);
            });

            var sessionId = session.Id!.Value;

            var sessionAuth = sessionAuthServices.SingleOrDefault();
            if (sessionAuth is not null && !await sessionAuth.Start(sessionId, user.Id!.Value, method.Value))
            {
                await sessionService.End(new(sessionId));
                return new() { Result = AuthResult.Results.AccessCodeDenied };
            }

            sessionFetcher.Invalidate(sessionId);

            return new()
            {
                Result = AuthResult.Results.AccessCodeAccepted,
                SessionId = sessionId,
                ResetPassword = method == NativeAuthMethod.PasswordEmailCode && user.ResetPassword == true,
            };
        });
    }

    public async Task<Response> ResetPassword(Request<AccessCode> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var accessCode = request.Value;

            if (accessCode.Username.HasNothing())
            {
                response.AddError("Username is required.");
                return;
            }

            var user = await UserSelectByUsername.Execute(Connection, accessCode.PortalId, accessCode.Username!);

            if (user is null)
                return;

            var method = await authPolicy.Resolve(user.Id!.Value);

            if (method is not NativeAuthMethod.PasswordEmailCode and not NativeAuthMethod.EmailCode)
                return;

            if (method == NativeAuthMethod.PasswordEmailCode)
                await UserUpdateResetPassword.Execute(Connection, request.SessionId, user.Id);

            var accessCodeResponse = await accessCodeService.Generate(new(request.SessionId, user));

            if (!accessCodeResponse.Ok)
                response.AddErrors(accessCodeResponse.Errors);
        });
    }

    public async Task<Response> ChangePassword(Request<PasswordChange> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var passwordChange = request.Value;
            var user = await UserSelectPassword.Execute(Connection, request.SessionId);

            if (user is null)
            {
                response.AddError("Account not found.");
                return;
            }

            if (await authPolicy.Resolve(user.Id!.Value) != NativeAuthMethod.PasswordEmailCode)
            {
                response.AddError("Password changes are not available for this account.");
                return;
            }

            user.PasswordSalt = cryptographyService.GetRandomSalt();
            user.PasswordHash = cryptographyService.ComputeHash(passwordChange.NewPassword!, user.PasswordSalt);

            await UserUpdatePassword.Execute(Connection, request.SessionId, user);
        });
    }

    public async Task<Response> SignOut(Request request)
    {
        var sessionAuth = sessionAuthServices.SingleOrDefault();
        if (sessionAuth is not null
            && request.SessionId is Guid sessionId
            && await sessionAuth.Revoke(sessionId, "signed-out"))
            return new();

        return await sessionService.End(request);
    }

}