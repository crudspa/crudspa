namespace Crudspa.Samples.Composer.Server.Services;

public class ComposerContactServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    IUserRepository userRepository,
    IContactRepository contactRepository)
    : IComposerContactService
{
    private static readonly Guid PortalId = new("aea2c861-459a-490c-b7c3-30e5156fec9f");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ComposerContact>>> Search(Request<ComposerContactSearch> request)
    {
        return await wrappers.Try<IList<ComposerContact>>(request, async response =>
        {
            var composerContacts = await ComposerContactSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, composerContacts.Select(x => x.ContactId), PortalId);

            foreach (var composerContact in composerContacts)
                composerContact.Contact = contacts.First(x => x.Id.Equals(composerContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, composerContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var composerContact in composerContacts.Where(x => x.UserId is not null))
                composerContact.User = users.FirstOrDefault(x => x.Id.Equals(composerContact.UserId), new());

            return composerContacts;
        });
    }

    public async Task<Response<ComposerContact?>> Fetch(Request<ComposerContact> request)
    {
        return await wrappers.Try<ComposerContact?>(request, async response =>
        {
            var composerContact = await ComposerContactSelect.Execute(Connection, request.SessionId, request.Value);

            composerContact?.Contact = await contactRepository.Select(Connection, composerContact.ContactId, PortalId) ?? new();

            if (composerContact?.UserId is not null)
                composerContact?.User = await userRepository.Select(Connection, composerContact.UserId, PortalId) ?? new();

            return composerContact;
        });
    }

    public async Task<Response<ComposerContact?>> Add(Request<ComposerContact> request)
    {
        return await wrappers.Validate<ComposerContact?, ComposerContact>(request, async response =>
        {
            var composerContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, composerContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, composerContact.User, PortalId));

            if (response.Errors.HasItems())
                return null;

            UserContactEmailSync.Apply(composerContact.Contact, composerContact.User);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                composerContact.ContactId = await contactRepository.Insert(connection, transaction, request.SessionId, composerContact.Contact, PortalId);

                composerContact.User.Contact.Id = composerContact.ContactId;
                composerContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                composerContact.UserId = await userRepository.Insert(connection, transaction, request.SessionId, composerContact.User, PortalId);

                var id = await ComposerContactInsert.Execute(connection, transaction, request.SessionId, composerContact);

                return new ComposerContact
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ComposerContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var composerContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, composerContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, composerContact.User, PortalId));

            if (response.Errors.HasItems())
                return;

            var existingContact = await contactRepository.Select(Connection, composerContact.ContactId, PortalId);
            var existingUser = composerContact.UserId is null ? null : await userRepository.Select(Connection, composerContact.UserId, PortalId);
            UserContactEmailSync.Apply(composerContact.Contact, composerContact.User, existingContact, existingUser);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await contactRepository.Update(connection, transaction, request.SessionId, composerContact.Contact, PortalId);

                composerContact.User.Contact.Id = composerContact.ContactId;
                composerContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                composerContact.UserId = await userRepository.Update(connection, transaction, request.SessionId, composerContact.User, PortalId);

                await ComposerContactUpdate.Execute(connection, transaction, request.SessionId, composerContact);
            });
        });
    }

    public async Task<Response> Remove(Request<ComposerContact> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var composerContact = request.Value;
            var existing = await ComposerContactSelect.Execute(Connection, request.SessionId, composerContact);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ComposerContactDelete.Execute(connection, transaction, request.SessionId, composerContact);

                await contactRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.ContactId });

                if (existing.UserId is not null)
                    await userRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.UserId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ComposerSelectRoleNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response> SendAccessCode(Request<ComposerContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var composerContact = await ComposerContactSelect.Execute(Connection, request.SessionId, request.Value);

            if (composerContact is null)
                throw new("Composer contact not found.");

            if (composerContact.UserId.HasNothing())
                throw new("User account not found.");

            var accessCodeRequest = new Request<User>(request.SessionId, new() { Id = composerContact.UserId });
            var accessCodeResponse = await accessCodeService.Generate(accessCodeRequest);
            response.AddErrors(accessCodeResponse.Errors);
        });
    }

    private async Task<Guid?> FetchOrganizationId(Guid? sessionId)
    {
        var composer = await ComposerSelect.Execute(Connection, sessionId);
        return composer?.OrganizationId;
    }
}