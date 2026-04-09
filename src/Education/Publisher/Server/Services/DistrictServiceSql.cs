namespace Crudspa.Education.Publisher.Server.Services;

public class DistrictServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : IDistrictService
{
    private static readonly Guid PortalId = new("18da2a92-c650-42fb-8ff9-07c81ab5b9b2");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<District>>> Search(Request<DistrictSearch> request)
    {
        return await wrappers.Try<IList<District>>(request, async response =>
        {
            var districts = await DistrictSelectWhere.Execute(Connection, request.SessionId, request.Value);

            var organizations = await organizationRepository.SelectByIds(Connection, districts.Select(x => x.OrganizationId), PortalId);

            foreach (var district in districts)
                district.Organization = organizations.First(x => x.Id.Equals(district.OrganizationId));

            var usaPostals = await UsaPostalRepositorySql.SelectByIds(Connection, districts.Where(x => x.AddressId is not null).Select(x => x.AddressId), PortalId);

            foreach (var district in districts.Where(x => x.AddressId is not null))
                district.UsaPostal = usaPostals.FirstOrDefault(x => x.Id.Equals(district.AddressId), new());

            return districts;
        });
    }

    public async Task<Response<District?>> Fetch(Request<District> request)
    {
        return await wrappers.Try<District?>(request, async response =>
        {
            var district = await DistrictSelect.Execute(Connection, request.SessionId, request.Value);

            district?.Organization = await organizationRepository.Select(Connection, district.OrganizationId, PortalId) ?? new();

            if (district?.AddressId is not null)
                district?.UsaPostal = await UsaPostalRepositorySql.Select(Connection, district.AddressId, PortalId) ?? new();

            return district;
        });
    }

    public async Task<Response<District?>> Add(Request<District> request)
    {
        return await wrappers.Validate<District?, District>(request, async response =>
        {
            var district = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, district.Organization, PortalId));
            response.AddErrors(await UsaPostalRepositorySql.Validate(Connection, district.UsaPostal, PortalId));

            if (response.Errors.HasItems())
                return null;

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                district.OrganizationId = await organizationRepository.Insert(connection, transaction, request.SessionId, district.Organization, PortalId);

                district.AddressId = await UsaPostalRepositorySql.Insert(connection, transaction, request.SessionId, district.UsaPostal, PortalId);

                var id = await DistrictInsert.Execute(connection, transaction, request.SessionId, district);

                return new District
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<District> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var district = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, district.Organization, PortalId));
            response.AddErrors(await UsaPostalRepositorySql.Validate(Connection, district.UsaPostal, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, district.Organization, PortalId);

                await UsaPostalRepositorySql.Update(connection, transaction, request.SessionId, district.UsaPostal, PortalId);

                district.AddressId ??= district.UsaPostal.Id;

                await DistrictUpdate.Execute(connection, transaction, request.SessionId, district);
            });
        });
    }

    public async Task<Response> Remove(Request<District> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var district = request.Value;
            var existing = await DistrictSelect.Execute(Connection, request.SessionId, district);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await DistrictDelete.Execute(connection, transaction, request.SessionId, district);

                await organizationRepository.Delete(connection, transaction, request.SessionId, new() { Id = existing.OrganizationId });

                if (existing.AddressId is not null)
                    await UsaPostalRepositorySql.Delete(connection, transaction, request.SessionId, new() { Id = existing.AddressId });
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}