namespace Crudspa.Samples.Catalog.Server.Services;

public class AuthServiceSqlCatalog(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionService sessionService,
    ISessionFetcher sessionFetcher,
    IUserRepository userRepository,
    IContactRepository contactRepository)
    : IAuthService
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public async Task<Response<AuthResult?>> CheckCredentials(Request<Credentials> request)
    {
        return await wrappers.Try<AuthResult?>(request, async response =>
        {
            var name = request.Value.Username?.Trim();

            if (name.HasNothing())
            {
                response.AddError("Name is required.");
                return new() { Result = AuthResult.Results.CredentialsInvalid };
            }

            if (name!.Length > 75)
            {
                response.AddError("Name cannot be longer than 75 characters.");
                return new() { Result = AuthResult.Results.CredentialsInvalid };
            }

            if (request.SessionId is not { } sessionId)
                return new() { Result = AuthResult.Results.SessionNotStarted };

            var sessionResponse = await sessionService.IsValidForSignIn(new(new() { Id = sessionId }));

            if (!sessionResponse.Ok)
                return new() { Result = AuthResult.Results.SessionNotStarted };

            var organizationId = await CatalogSelectOrganizationId.Execute(Connection);

            if (organizationId is null)
            {
                response.AddError("Catalog sample organization not found.");
                return new() { Result = AuthResult.Results.CredentialsInvalid };
            }

            var roles = (await CatalogSelectRoleNames.Execute(Connection, sessionId))
                .Select(x => new Selectable
                {
                    Id = x.Id,
                    Name = x.Name,
                    Selected = true,
                })
                .ToObservable();

            if (roles.IsEmpty())
            {
                response.AddError("Catalog sample roles not found.");
                return new() { Result = AuthResult.Results.CredentialsInvalid };
            }

            var (firstName, lastName) = SplitName(name);

            var contact = new Contact
            {
                FirstName = firstName,
                LastName = lastName,
                TimeZoneId = Constants.DefaultTimeZone,
            };

            var user = new User
            {
                Contact = contact,
                OrganizationId = organizationId,
                Username = $"catalog-{sessionId:N}",
                ResetPassword = false,
                MaySignIn = true,
                Roles = roles,
            };

            response.AddErrors(await contactRepository.Validate(Connection, contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, user, PortalId));

            if (response.Errors.HasItems())
                return new() { Result = AuthResult.Results.CredentialsInvalid };

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var contactId = await contactRepository.Insert(connection, transaction, sessionId, contact, PortalId);
                contact.Id = contactId;

                var userId = await userRepository.Insert(connection, transaction, sessionId, user, PortalId);

                await CatalogContactInsert.Execute(connection, transaction, sessionId, new()
                {
                    ContactId = contactId,
                    UserId = userId,
                });

                await SessionUpdateUser.Execute(connection, transaction, sessionId, userId);
            });

            sessionFetcher.Invalidate(sessionId);

            return new()
            {
                Result = AuthResult.Results.CredentialsCorrect,
                SessionId = sessionId,
            };
        });
    }

    public Task<Response<AuthResult?>> CheckAccessCode(Request<AccessCode> request) =>
        Task.FromResult(new Response<AuthResult?>("Catalog sample sign-in does not use access codes."));

    public Task<Response> ResetPassword(Request<AccessCode> request) =>
        Task.FromResult(new Response("Catalog sample sign-in does not use password resets."));

    public Task<Response> ChangePassword(Request<PasswordChange> request) =>
        Task.FromResult(new Response("Catalog sample sign-in does not use passwords."));

    public async Task<Response> SignOut(Request request) =>
        await sessionService.End(request);

    private static (String FirstName, String? LastName) SplitName(String name)
    {
        var collapsed = String.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        var comma = collapsed.IndexOf(',');

        if (comma > 0)
        {
            var lastName = collapsed[..comma].Trim();
            var firstName = collapsed[(comma + 1)..].Trim();

            if (firstName.HasSomething() && lastName.HasSomething())
                return (firstName, lastName);
        }

        var parts = collapsed.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return parts.Length switch
        {
            0 => (String.Empty, null),
            1 => (parts[0], null),
            _ => (String.Join(" ", parts[..^1]), parts[^1]),
        };
    }
}