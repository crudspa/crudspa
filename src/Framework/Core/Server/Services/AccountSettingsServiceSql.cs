namespace Crudspa.Framework.Core.Server.Services;

public class AccountSettingsServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IContactRepository contactRepository)
    : IAccountSettingsService
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public async Task<Response<User?>> Fetch(Request request)
    {
        return await wrappers.Try<User?>(request, async response =>
            await UserSelectBySession.Execute(Connection, request.SessionId, PortalId));
    }

    public async Task<Response> Save(Request<User> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var user = request.Value;

            var usernameIsAvailable = await UserUsernameIsAvailable.Execute(Connection, user.Username, user.PortalId, user.Id);

            if (!usernameIsAvailable)
            {
                response.AddError("Username is unavailable.");
                return;
            }

            var existingUser = await UserSelectBySession.Execute(Connection, request.SessionId, PortalId);

            if (existingUser?.Username is null || user.Username is null)
                throw new("Username missing.");

            var contactId = existingUser.Contact.Id;

            if (contactId.HasNothing())
                throw new("ContactId missing.");

            var existingContact = await contactRepository.Select(Connection, contactId, PortalId) ?? new() { Id = contactId };

            UserContactEmailSync.Apply(existingContact, user, existingContact, existingUser);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await UserUpdateSettings.Execute(connection, transaction, request.SessionId, user);

                await contactRepository.Update(connection, transaction, request.SessionId, existingContact, PortalId);
            });
        });
    }
}