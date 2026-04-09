namespace Crudspa.Education.Publisher.Server.Services;

public class SchoolContactServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    IUserRepository userRepository,
    IContactRepository contactRepository)
    : ISchoolContactService
{
    private static readonly Guid PortalId = new("c882bec5-cca6-4327-8f37-7729b2839b80");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SchoolContact>>> Search(Request<SchoolContactSearch> request)
    {
        return await wrappers.Try<IList<SchoolContact>>(request, async response =>
        {
            var schoolContacts = await SchoolContactSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, schoolContacts.Select(x => x.ContactId), PortalId);

            foreach (var schoolContact in schoolContacts)
                schoolContact.Contact = contacts.First(x => x.Id.Equals(schoolContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, schoolContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var schoolContact in schoolContacts.Where(x => x.UserId is not null))
                schoolContact.User = users.FirstOrDefault(x => x.Id.Equals(schoolContact.UserId), new());

            return schoolContacts;
        });
    }

    public async Task<Response<IList<SchoolContact>>> SearchForSchool(Request<SchoolContactSearch> request)
    {
        return await wrappers.Try<IList<SchoolContact>>(request, async response =>
        {
            var schoolContacts = await SchoolContactSelectWhereForSchool.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, schoolContacts.Select(x => x.ContactId), PortalId);

            foreach (var schoolContact in schoolContacts)
                schoolContact.Contact = contacts.First(x => x.Id.Equals(schoolContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, schoolContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var schoolContact in schoolContacts.Where(x => x.UserId is not null))
                schoolContact.User = users.FirstOrDefault(x => x.Id.Equals(schoolContact.UserId), new());

            return schoolContacts;
        });
    }

    public async Task<Response<SchoolContact?>> Fetch(Request<SchoolContact> request)
    {
        return await wrappers.Try<SchoolContact?>(request, async response =>
        {
            var schoolContact = await SchoolContactSelect.Execute(Connection, request.SessionId, request.Value);

            schoolContact?.Contact = await contactRepository.Select(Connection, schoolContact.ContactId, PortalId) ?? new();

            if (schoolContact?.UserId is not null)
                schoolContact?.User = await userRepository.Select(Connection, schoolContact.UserId, PortalId) ?? new();

            return schoolContact;
        });
    }

    public async Task<Response<SchoolContact?>> Add(Request<SchoolContact> request)
    {
        return await wrappers.Validate<SchoolContact?, SchoolContact>(request, async response =>
        {
            var schoolContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, schoolContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, schoolContact.User, PortalId));

            if (response.Errors.HasItems())
                return null;

            UserContactEmailSync.Apply(schoolContact.Contact, schoolContact.User);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                schoolContact.ContactId = await contactRepository.Insert(connection, transaction, request.SessionId, schoolContact.Contact, PortalId);

                schoolContact.User.Contact.Id = schoolContact.ContactId;
                schoolContact.User.OrganizationId = await FetchOrganizationId(request.SessionId, schoolContact.SchoolId);
                schoolContact.UserId = await userRepository.Insert(connection, transaction, request.SessionId, schoolContact.User, PortalId);

                var id = await SchoolContactInsert.Execute(connection, transaction, request.SessionId, schoolContact);

                return new SchoolContact
                {
                    Id = id,
                    SchoolId = schoolContact.SchoolId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<SchoolContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var schoolContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, schoolContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, schoolContact.User, PortalId));

            if (response.Errors.HasItems())
                return;

            var existingContact = await contactRepository.Select(Connection, schoolContact.ContactId, PortalId);
            var existingUser = schoolContact.UserId is null ? null : await userRepository.Select(Connection, schoolContact.UserId, PortalId);
            UserContactEmailSync.Apply(schoolContact.Contact, schoolContact.User, existingContact, existingUser);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await contactRepository.Update(connection, transaction, request.SessionId, schoolContact.Contact, PortalId);

                schoolContact.User.Contact.Id = schoolContact.ContactId;
                schoolContact.User.OrganizationId = await FetchOrganizationId(request.SessionId, schoolContact.SchoolId);
                schoolContact.UserId = await userRepository.Update(connection, transaction, request.SessionId, schoolContact.User, PortalId);

                await SchoolContactUpdate.Execute(connection, transaction, request.SessionId, schoolContact);
            });
        });
    }

    public async Task<Response> Remove(Request<SchoolContact> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var schoolContact = request.Value;
            var existing = await SchoolContactSelect.Execute(Connection, request.SessionId, schoolContact);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await SchoolContactDelete.Execute(connection, transaction, request.SessionId, schoolContact);

                await contactRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.ContactId });

                if (existing.UserId is not null)
                    await userRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.UserId });
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchTitleNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await TitleSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchRoleNames(Request<School> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await SchoolSelectRoleNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response> SendAccessCode(Request<SchoolContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var schoolContact = await SchoolContactSelect.Execute(Connection, request.SessionId, request.Value);

            if (schoolContact is null)
                throw new("School contact not found.");

            if (schoolContact.UserId.HasNothing())
                throw new("User account not found.");

            var accessCodeRequest = new Request<User>(request.SessionId, new() { Id = schoolContact.UserId });
            var accessCodeResponse = await accessCodeService.Generate(accessCodeRequest);
            response.AddErrors(accessCodeResponse.Errors);
        });
    }

    private async Task<Guid?> FetchOrganizationId(Guid? sessionId, Guid? schoolId)
    {
        var school = await SchoolSelect.Execute(Connection, sessionId, new() { Id = schoolId });
        return school?.OrganizationId;
    }

    public async Task<Response<SchoolContact>> FetchRelations(Request<SchoolContact> request)
    {
        return await wrappers.Try<SchoolContact>(request, async response =>
        {
            var schoolContact = await SchoolContactSelectRelations.Execute(Connection, request.Value);
            return schoolContact;
        });
    }

    public async Task<Response> SaveRelations(Request<SchoolContact> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var schoolContact = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SchoolContactUpdateRelations.Execute(connection, transaction, request.SessionId, schoolContact);
            });
        });
    }
}