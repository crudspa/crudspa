namespace Crudspa.Education.Provider.Server.Services;

public class ProviderContactServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    IUserRepository userRepository,
    IContactRepository contactRepository)
    : IProviderContactService
{
    private static readonly Guid PortalId = new("2f6b54e3-689f-46a3-a1ee-30a4bfe18d63");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ProviderContact>>> Search(Request<ProviderContactSearch> request)
    {
        return await wrappers.Try<IList<ProviderContact>>(request, async response =>
        {
            var providerContacts = await ProviderContactSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, providerContacts.Select(x => x.ContactId), PortalId);

            foreach (var providerContact in providerContacts)
                providerContact.Contact = contacts.First(x => x.Id.Equals(providerContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, providerContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var providerContact in providerContacts.Where(x => x.UserId is not null))
                providerContact.User = users.FirstOrDefault(x => x.Id.Equals(providerContact.UserId), new());

            return providerContacts;
        });
    }

    public async Task<Response<ProviderContact?>> Fetch(Request<ProviderContact> request)
    {
        return await wrappers.Try<ProviderContact?>(request, async response =>
        {
            var providerContact = await ProviderContactSelect.Execute(Connection, request.SessionId, request.Value);

            providerContact?.Contact = await contactRepository.Select(Connection, providerContact.ContactId, PortalId) ?? new();

            if (providerContact?.UserId is not null)
                providerContact?.User = await userRepository.Select(Connection, providerContact.UserId, PortalId) ?? new();

            return providerContact;
        });
    }

    public async Task<Response<ProviderContact?>> Add(Request<ProviderContact> request)
    {
        return await wrappers.Validate<ProviderContact?, ProviderContact>(request, async response =>
        {
            var providerContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, providerContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, providerContact.User, PortalId));

            if (response.Errors.HasItems())
                return null;

            UserContactEmailSync.Apply(providerContact.Contact, providerContact.User);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                providerContact.ContactId = await contactRepository.Insert(connection, transaction, request.SessionId, providerContact.Contact, PortalId);

                providerContact.User.Contact.Id = providerContact.ContactId;
                providerContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                providerContact.UserId = await userRepository.Insert(connection, transaction, request.SessionId, providerContact.User, PortalId);

                var id = await ProviderContactInsert.Execute(connection, transaction, request.SessionId, providerContact);

                return new ProviderContact
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ProviderContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var providerContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, providerContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, providerContact.User, PortalId));

            if (response.Errors.HasItems())
                return;

            var existingContact = await contactRepository.Select(Connection, providerContact.ContactId, PortalId);
            var existingUser = providerContact.UserId is null ? null : await userRepository.Select(Connection, providerContact.UserId, PortalId);
            UserContactEmailSync.Apply(providerContact.Contact, providerContact.User, existingContact, existingUser);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await contactRepository.Update(connection, transaction, request.SessionId, providerContact.Contact, PortalId);

                providerContact.User.Contact.Id = providerContact.ContactId;
                providerContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                providerContact.UserId = await userRepository.Update(connection, transaction, request.SessionId, providerContact.User, PortalId);

                await ProviderContactUpdate.Execute(connection, transaction, request.SessionId, providerContact);
            });
        });
    }

    public async Task<Response> Remove(Request<ProviderContact> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var providerContact = request.Value;
            var existing = await ProviderContactSelect.Execute(Connection, request.SessionId, providerContact);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ProviderContactDelete.Execute(connection, transaction, request.SessionId, providerContact);

                await contactRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.ContactId });

                if (existing.UserId is not null)
                    await userRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.UserId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ProviderSelectRoleNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response> SendAccessCode(Request<ProviderContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var providerContact = await ProviderContactSelect.Execute(Connection, request.SessionId, request.Value);

            if (providerContact is null)
                throw new("Provider contact not found.");

            if (providerContact.UserId.HasNothing())
                throw new("User account not found.");

            var accessCodeRequest = new Request<User>(request.SessionId, new() { Id = providerContact.UserId });
            var accessCodeResponse = await accessCodeService.Generate(accessCodeRequest);
            response.AddErrors(accessCodeResponse.Errors);
        });
    }

    private async Task<Guid?> FetchOrganizationId(Guid? sessionId)
    {
        var provider = await ProviderSelect.Execute(Connection, sessionId);
        return provider?.OrganizationId;
    }
}