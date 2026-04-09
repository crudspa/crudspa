namespace Crudspa.Framework.Core.Server.Services;

public class AuthServiceSqlEmailTfa(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    ISessionService sessionService,
    ICryptographyService cryptographyService,
    ISessionFetcher sessionFetcher)
    : IAuthService
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public async Task<Response<AuthResult?>> CheckCredentials(Request<Credentials> request)
    {
        return await wrappers.Try<AuthResult?>(request, async response =>
        {
            var credentials = request.Value;

            response.Errors = credentials.Validate();

            if (response.Errors.HasItems())
                return new() { Result = AuthResult.Results.CredentialsInvalid };

            var session = new Session { Id = request.SessionId };
            var sessionResponse = await sessionService.IsValidForSignIn(new(session));

            if (!sessionResponse.Ok)
            {
                var sessionCreateResponse = await sessionService.FetchOrCreate(new(new()));

                if (!sessionCreateResponse.Ok)
                    return new() { Result = AuthResult.Results.SessionNotStarted };

                session.Id = sessionCreateResponse.Value.Id;
            }

            var user = await UserSelectByUsername.Execute(Connection, PortalId, credentials.Username!);

            if (user is null)
                return new() { Result = AuthResult.Results.CredentialsIncorrect };

            if (user.PasswordHash is null || user.PasswordSalt is null)
            {
                await accessCodeService.Generate(new(request.SessionId, user));
                return new() { Result = AuthResult.Results.PasswordNotSet };
            }

            var hash = cryptographyService.ComputeHash(credentials.Password!, user.PasswordSalt);

            if (!hash.SequenceEqual(user.PasswordHash))
                return new() { Result = AuthResult.Results.CredentialsIncorrect };

            await accessCodeService.Generate(new(request.SessionId, user));

            return new() { Result = AuthResult.Results.CredentialsCorrect };
        });
    }

    public async Task<Response<AuthResult?>> CheckAccessCode(Request<AccessCode> request)
    {
        return await wrappers.Try<AuthResult?>(request, async response =>
        {
            var accessCode = request.Value;

            if (accessCode.Username.HasNothing())
                return new() { Result = AuthResult.Results.CredentialsInvalid };

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

            var success = await AccessCodeUpdate.Execute(Connection, user.Id, accessCode);

            if (!success)
                return new() { Result = AuthResult.Results.AccessCodeDenied };

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SessionUpdateUser.Execute(connection, transaction, session.Id, user.Id);
            });

            var sessionId = session.Id!.Value;
            sessionFetcher.Invalidate(sessionId);

            return new()
            {
                Result = AuthResult.Results.AccessCodeAccepted,
                SessionId = sessionId,
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
            {
                response.AddError("Account not found.");
                return;
            }

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

            user.PasswordSalt = cryptographyService.GetRandomSalt();
            user.PasswordHash = cryptographyService.ComputeHash(passwordChange.NewPassword!, user.PasswordSalt);

            await UserUpdatePassword.Execute(Connection, request.SessionId, user);
        });
    }

    public async Task<Response> SignOut(Request request)
    {
        return await sessionService.End(request);
    }

}