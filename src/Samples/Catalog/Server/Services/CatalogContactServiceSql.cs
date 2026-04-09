namespace Crudspa.Samples.Catalog.Server.Services;

public class CatalogContactServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    IUserRepository userRepository,
    IContactRepository contactRepository)
    : ICatalogContactService
{
    private static readonly Guid PortalId = new("651a367c-a7dd-4fe8-be5a-b70ef275a8ec");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<CatalogContact>>> Search(Request<CatalogContactSearch> request)
    {
        return await wrappers.Try<IList<CatalogContact>>(request, async response =>
        {
            var catalogContacts = await CatalogContactSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, catalogContacts.Select(x => x.ContactId), PortalId);

            foreach (var catalogContact in catalogContacts)
                catalogContact.Contact = contacts.First(x => x.Id.Equals(catalogContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, catalogContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var catalogContact in catalogContacts.Where(x => x.UserId is not null))
                catalogContact.User = users.FirstOrDefault(x => x.Id.Equals(catalogContact.UserId), new());

            return catalogContacts;
        });
    }

    public async Task<Response<CatalogContact?>> Fetch(Request<CatalogContact> request)
    {
        return await wrappers.Try<CatalogContact?>(request, async response =>
        {
            var catalogContact = await CatalogContactSelect.Execute(Connection, request.SessionId, request.Value);

            catalogContact?.Contact = await contactRepository.Select(Connection, catalogContact.ContactId, PortalId) ?? new();

            if (catalogContact?.UserId is not null)
                catalogContact?.User = await userRepository.Select(Connection, catalogContact.UserId, PortalId) ?? new();

            return catalogContact;
        });
    }

    public async Task<Response<CatalogContact?>> Add(Request<CatalogContact> request)
    {
        return await wrappers.Validate<CatalogContact?, CatalogContact>(request, async response =>
        {
            var catalogContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, catalogContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, catalogContact.User, PortalId));

            if (response.Errors.HasItems())
                return null;

            UserContactEmailSync.Apply(catalogContact.Contact, catalogContact.User);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                catalogContact.ContactId = await contactRepository.Insert(connection, transaction, request.SessionId, catalogContact.Contact, PortalId);

                catalogContact.User.Contact.Id = catalogContact.ContactId;
                catalogContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                catalogContact.UserId = await userRepository.Insert(connection, transaction, request.SessionId, catalogContact.User, PortalId);

                var id = await CatalogContactInsert.Execute(connection, transaction, request.SessionId, catalogContact);

                return new CatalogContact
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<CatalogContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var catalogContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, catalogContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, catalogContact.User, PortalId));

            if (response.Errors.HasItems())
                return;

            var existingContact = await contactRepository.Select(Connection, catalogContact.ContactId, PortalId);
            var existingUser = catalogContact.UserId is null ? null : await userRepository.Select(Connection, catalogContact.UserId, PortalId);
            UserContactEmailSync.Apply(catalogContact.Contact, catalogContact.User, existingContact, existingUser);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await contactRepository.Update(connection, transaction, request.SessionId, catalogContact.Contact, PortalId);

                catalogContact.User.Contact.Id = catalogContact.ContactId;
                catalogContact.User.OrganizationId = await FetchOrganizationId(request.SessionId);
                catalogContact.UserId = await userRepository.Update(connection, transaction, request.SessionId, catalogContact.User, PortalId);

                await CatalogContactUpdate.Execute(connection, transaction, request.SessionId, catalogContact);
            });
        });
    }

    public async Task<Response> Remove(Request<CatalogContact> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var catalogContact = request.Value;
            var existing = await CatalogContactSelect.Execute(Connection, request.SessionId, catalogContact);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await CatalogContactDelete.Execute(connection, transaction, request.SessionId, catalogContact);

                await contactRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.ContactId });

                if (existing.UserId is not null)
                    await userRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.UserId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchRoleNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await CatalogSelectRoleNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response> SendAccessCode(Request<CatalogContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var catalogContact = await CatalogContactSelect.Execute(Connection, request.SessionId, request.Value);

            if (catalogContact is null)
                throw new("Catalog contact not found.");

            if (catalogContact.UserId.HasNothing())
                throw new("User account not found.");

            var accessCodeRequest = new Request<User>(request.SessionId, new() { Id = catalogContact.UserId });
            var accessCodeResponse = await accessCodeService.Generate(accessCodeRequest);
            response.AddErrors(accessCodeResponse.Errors);
        });
    }

    private async Task<Guid?> FetchOrganizationId(Guid? sessionId)
    {
        var catalog = await CatalogSelect.Execute(Connection, sessionId);
        return catalog?.OrganizationId;
    }
}