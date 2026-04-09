namespace Crudspa.Education.District.Server.Services;

public class SchoolServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : ISchoolService
{
    private static readonly Guid PortalId = new("c882bec5-cca6-4327-8f37-7729b2839b80");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<School>>> Search(Request<SchoolSearch> request)
    {
        return await wrappers.Try<IList<School>>(request, async response =>
        {
            var schools = await SchoolSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var organizations = await organizationRepository.SelectByIds(Connection, schools.Select(x => x.OrganizationId), PortalId);

            foreach (var school in schools)
                school.Organization = organizations.First(x => x.Id.Equals(school.OrganizationId));

            var usaPostals = await UsaPostalRepositorySql.SelectByIds(Connection, schools.Where(x => x.AddressId is not null).Select(x => x.AddressId), PortalId);

            foreach (var school in schools.Where(x => x.AddressId is not null))
                school.UsaPostal = usaPostals.FirstOrDefault(x => x.Id.Equals(school.AddressId), new());

            return schools;
        });
    }

    public async Task<Response<School?>> Fetch(Request<School> request)
    {
        return await wrappers.Try<School?>(request, async response =>
        {
            var school = await SchoolSelect.Execute(Connection, request.SessionId, request.Value);

            school?.Organization = await organizationRepository.Select(Connection, school.OrganizationId, PortalId) ?? new();

            if (school?.AddressId is not null)
                school?.UsaPostal = await UsaPostalRepositorySql.Select(Connection, school.AddressId, PortalId) ?? new();

            return school;
        });
    }

    public async Task<Response<School?>> Add(Request<School> request)
    {
        return await wrappers.Validate<School?, School>(request, async response =>
        {
            var school = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, school.Organization, PortalId));
            response.AddErrors(await UsaPostalRepositorySql.Validate(Connection, school.UsaPostal, PortalId));

            if (response.Errors.HasItems())
                return null;

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                school.OrganizationId = await organizationRepository.Insert(connection, transaction, request.SessionId, school.Organization, PortalId);

                school.AddressId = await UsaPostalRepositorySql.Insert(connection, transaction, request.SessionId, school.UsaPostal, PortalId);

                var id = await SchoolInsert.Execute(connection, transaction, request.SessionId, school);

                return new School
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<School> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var school = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, school.Organization, PortalId));
            response.AddErrors(await UsaPostalRepositorySql.Validate(Connection, school.UsaPostal, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, school.Organization, PortalId);

                await UsaPostalRepositorySql.Update(connection, transaction, request.SessionId, school.UsaPostal, PortalId);

                school.AddressId ??= school.UsaPostal.Id;

                await SchoolUpdate.Execute(connection, transaction, request.SessionId, school);
            });
        });
    }

    public async Task<Response> Remove(Request<School> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var school = request.Value;
            var existing = await SchoolSelect.Execute(Connection, request.SessionId, school);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await SchoolDelete.Execute(connection, transaction, request.SessionId, school);

                await organizationRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.OrganizationId });

                if (existing.AddressId is not null)
                    await UsaPostalRepositorySql.Delete(connection, transaction, request.SessionId, new() { Id = existing.AddressId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchCommunityNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await CommunitySelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}