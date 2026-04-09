namespace Crudspa.Education.Publisher.Server.Services;

public class DistrictContactServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAccessCodeService accessCodeService,
    IUserRepository userRepository,
    IContactRepository contactRepository)
    : IDistrictContactService
{
    private static readonly Guid PortalId = new("18da2a92-c650-42fb-8ff9-07c81ab5b9b2");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<DistrictContact>>> Search(Request<DistrictContactSearch> request)
    {
        return await wrappers.Try<IList<DistrictContact>>(request, async response =>
        {
            var districtContacts = await DistrictContactSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, districtContacts.Select(x => x.ContactId), PortalId);

            foreach (var districtContact in districtContacts)
                districtContact.Contact = contacts.First(x => x.Id.Equals(districtContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, districtContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var districtContact in districtContacts.Where(x => x.UserId is not null))
                districtContact.User = users.FirstOrDefault(x => x.Id.Equals(districtContact.UserId), new());

            return districtContacts;
        });
    }

    public async Task<Response<IList<DistrictContact>>> SearchForDistrict(Request<DistrictContactSearch> request)
    {
        return await wrappers.Try<IList<DistrictContact>>(request, async response =>
        {
            var districtContacts = await DistrictContactSelectWhereForDistrict.Execute(Connection, request.SessionId, request.Value);

            var contacts = await contactRepository.SelectByIds(Connection, districtContacts.Select(x => x.ContactId), PortalId);

            foreach (var districtContact in districtContacts)
                districtContact.Contact = contacts.First(x => x.Id.Equals(districtContact.ContactId));

            var users = await userRepository.SelectByIds(Connection, districtContacts.Where(x => x.UserId is not null).Select(x => x.UserId), PortalId);

            foreach (var districtContact in districtContacts.Where(x => x.UserId is not null))
                districtContact.User = users.FirstOrDefault(x => x.Id.Equals(districtContact.UserId), new());

            return districtContacts;
        });
    }

    public async Task<Response<DistrictContact?>> Fetch(Request<DistrictContact> request)
    {
        return await wrappers.Try<DistrictContact?>(request, async response =>
        {
            var districtContact = await DistrictContactSelect.Execute(Connection, request.SessionId, request.Value);

            districtContact?.Contact = await contactRepository.Select(Connection, districtContact.ContactId, PortalId) ?? new();

            if (districtContact?.UserId is not null)
                districtContact?.User = await userRepository.Select(Connection, districtContact.UserId, PortalId) ?? new();

            return districtContact;
        });
    }

    public async Task<Response<DistrictContact?>> Add(Request<DistrictContact> request)
    {
        return await wrappers.Validate<DistrictContact?, DistrictContact>(request, async response =>
        {
            var districtContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, districtContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, districtContact.User, PortalId));

            if (response.Errors.HasItems())
                return null;

            UserContactEmailSync.Apply(districtContact.Contact, districtContact.User);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                districtContact.ContactId = await contactRepository.Insert(connection, transaction, request.SessionId, districtContact.Contact, PortalId);

                districtContact.User.Contact.Id = districtContact.ContactId;
                districtContact.User.OrganizationId = await FetchOrganizationId(request.SessionId, districtContact.DistrictId);
                districtContact.UserId = await userRepository.Insert(connection, transaction, request.SessionId, districtContact.User, PortalId);

                var id = await DistrictContactInsert.Execute(connection, transaction, request.SessionId, districtContact);

                return new DistrictContact
                {
                    Id = id,
                    DistrictId = districtContact.DistrictId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<DistrictContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var districtContact = request.Value;

            response.AddErrors(await contactRepository.Validate(Connection, districtContact.Contact, PortalId));
            response.AddErrors(await userRepository.Validate(Connection, districtContact.User, PortalId));

            if (response.Errors.HasItems())
                return;

            var existingContact = await contactRepository.Select(Connection, districtContact.ContactId, PortalId);
            var existingUser = districtContact.UserId is null ? null : await userRepository.Select(Connection, districtContact.UserId, PortalId);
            UserContactEmailSync.Apply(districtContact.Contact, districtContact.User, existingContact, existingUser);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await contactRepository.Update(connection, transaction, request.SessionId, districtContact.Contact, PortalId);

                districtContact.User.Contact.Id = districtContact.ContactId;
                districtContact.User.OrganizationId = await FetchOrganizationId(request.SessionId, districtContact.DistrictId);
                districtContact.UserId = await userRepository.Update(connection, transaction, request.SessionId, districtContact.User, PortalId);

                await DistrictContactUpdate.Execute(connection, transaction, request.SessionId, districtContact);
            });
        });
    }

    public async Task<Response> Remove(Request<DistrictContact> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var districtContact = request.Value;
            var existing = await DistrictContactSelect.Execute(Connection, request.SessionId, districtContact);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await DistrictContactDelete.Execute(connection, transaction, request.SessionId, districtContact);

                await contactRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.ContactId });

                if (existing.UserId is not null)
                    await userRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.UserId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchDistrictNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await DistrictSelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchRoleNames(Request<District> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await DistrictSelectRoleNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response> SendAccessCode(Request<DistrictContact> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var districtContact = await DistrictContactSelect.Execute(Connection, request.SessionId, request.Value);

            if (districtContact is null)
                throw new("District contact not found.");

            if (districtContact.UserId.HasNothing())
                throw new("User account not found.");

            var accessCodeRequest = new Request<User>(request.SessionId, new() { Id = districtContact.UserId });
            var accessCodeResponse = await accessCodeService.Generate(accessCodeRequest);
            response.AddErrors(accessCodeResponse.Errors);
        });
    }

    private async Task<Guid?> FetchOrganizationId(Guid? sessionId, Guid? districtId)
    {
        var district = await DistrictSelect.Execute(Connection, sessionId, new() { Id = districtId });
        return district?.OrganizationId;
    }
}