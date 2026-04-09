namespace Crudspa.Education.District.Server.Services;

using District = Shared.Contracts.Data.District;

public class DistrictServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : IDistrictService
{
    private static readonly Guid PortalId = new("18da2a92-c650-42fb-8ff9-07c81ab5b9b2");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<District?>> Fetch(Request request)
    {
        return await wrappers.Try<District?>(request, async response =>
        {
            var district = await DistrictSelect.Execute(Connection, request.SessionId);

            district?.Organization = await organizationRepository.Select(Connection, district.OrganizationId, PortalId) ?? new();

            return district;
        });
    }

    public async Task<Response> Save(Request<District> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var district = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, district.Organization, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, district.Organization, PortalId);
                await DistrictUpdate.Execute(connection, transaction, request.SessionId, district);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }

    public async Task<Response<IList<Named>>> FetchCommunityNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await CommunitySelectNames.Execute(Connection, request.SessionId));
    }
}