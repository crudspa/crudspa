namespace Crudspa.Education.Publisher.Server.Services;

public class PublisherContactServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    IUserRepository userRepository,
    IContactRepository contactRepository)
    : IPublisherContactService
{
    private static readonly Guid PortalId = new("de5e3ff9-ecdd-47d4-b05c-c61cde8994d4");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<PublisherContact>>> Search(Request<PublisherContactSearch> request)
    {
        return await wrappers.Try<IList<PublisherContact>>(request, async response =>
        {
            var publisherContacts = await PublisherContactSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, publisherContacts.Select(x => x.ContactId), PortalId);

            foreach (var publisherContact in publisherContacts)
                publisherContact.Contact = contacts.First(x => x.Id.Equals(publisherContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, publisherContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var publisherContact in publisherContacts.Where(x => x.UserId is not null))
                publisherContact.User = users.FirstOrDefault(x => x.Id.Equals(publisherContact.UserId), new());

            return publisherContacts;
        });
    }

    public async Task<Response<PublisherContact?>> Fetch(Request<PublisherContact> request)
    {
        return await wrappers.Try<PublisherContact?>(request, async response =>
        {
            var publisherContact = await PublisherContactSelect.Execute(Connection, request.SessionId, request.Value);

            publisherContact?.Contact = await contactRepository.Select(Connection, publisherContact.ContactId, PortalId) ?? new();

            if (publisherContact?.UserId is not null)
                publisherContact?.User = await userRepository.Select(Connection, publisherContact.UserId, PortalId) ?? new();

            return publisherContact;
        });
    }

    public async Task<Response<PublisherContact?>> Add(Request<PublisherContact> request)
    {
        return await wrappers.Validate<PublisherContact?, PublisherContact>(request, async response =>
        {
            var publisherContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, publisherContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, publisherContact.User, PortalId));

            if (response.Errors.HasItems())
                return null;

            UserContactEmailSync.Apply(publisherContact.Contact, publisherContact.User);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                publisherContact.ContactId = await contactRepository.Insert(connection, transaction, request.SessionId, publisherContact.Contact, PortalId);

                publisherContact.User.Contact.Id = publisherContact.ContactId;
                publisherContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                publisherContact.UserId = await userRepository.Insert(connection, transaction, request.SessionId, publisherContact.User, PortalId);

                var id = await PublisherContactInsert.Execute(connection, transaction, request.SessionId, publisherContact);

                return new PublisherContact
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<PublisherContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var publisherContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, publisherContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, publisherContact.User, PortalId));

            if (response.Errors.HasItems())
                return;

            var existingContact = await contactRepository.Select(Connection, publisherContact.ContactId, PortalId);
            var existingUser = publisherContact.UserId is null ? null : await userRepository.Select(Connection, publisherContact.UserId, PortalId);
            UserContactEmailSync.Apply(publisherContact.Contact, publisherContact.User, existingContact, existingUser);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await contactRepository.Update(connection, transaction, request.SessionId, publisherContact.Contact, PortalId);

                publisherContact.User.Contact.Id = publisherContact.ContactId;
                publisherContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                publisherContact.UserId = await userRepository.Update(connection, transaction, request.SessionId, publisherContact.User, PortalId);

                await PublisherContactUpdate.Execute(connection, transaction, request.SessionId, publisherContact);
            });
        });
    }

    public async Task<Response> Remove(Request<PublisherContact> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var publisherContact = request.Value;
            var existing = await PublisherContactSelect.Execute(Connection, request.SessionId, publisherContact);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await PublisherContactDelete.Execute(connection, transaction, request.SessionId, publisherContact);

                await contactRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.ContactId });

                if (existing.UserId is not null)
                    await userRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.UserId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PublisherSelectRoleNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response> SendAccessCode(Request<PublisherContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var publisherContact = await PublisherContactSelect.Execute(Connection, request.SessionId, request.Value);

            if (publisherContact is null)
                throw new("Publisher contact not found.");

            if (publisherContact.UserId.HasNothing())
                throw new("User account not found.");

            var accessCodeRequest = new Request<User>(request.SessionId, new() { Id = publisherContact.UserId });
            var accessCodeResponse = await accessCodeService.Generate(accessCodeRequest);
            response.AddErrors(accessCodeResponse.Errors);
        });
    }

    private async Task<Guid?> FetchOrganizationId(Guid? sessionId)
    {
        var publisher = await PublisherSelect.Execute(Connection, sessionId);
        return publisher?.OrganizationId;
    }
}