namespace Crudspa.Education.School.Server.Services;

using School = Shared.Contracts.Data.School;

public class SchoolServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IOrganizationRepository organizationRepository)
    : ISchoolService
{
    private static readonly Guid PortalId = new("c882bec5-cca6-4327-8f37-7729b2839b80");

    private String Connection => configService.Fetch().Database;

    public async Task<Response<School?>> Fetch(Request request)
    {
        return await wrappers.Try<School?>(request, async response =>
        {
            var school = await SchoolSelect.Execute(Connection, request.SessionId);

            school?.Organization = await organizationRepository.Select(Connection, school.OrganizationId, PortalId) ?? new();

            return school;
        });
    }

    public async Task<Response> Save(Request<School> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var school = request.Value;

            response.AddErrors(await organizationRepository.Validate(Connection, school.Organization, PortalId));

            if (response.Errors.HasItems())
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await organizationRepository.Update(connection, transaction, request.SessionId, school.Organization, PortalId);
                await SchoolUpdate.Execute(connection, transaction, request.SessionId, school);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchPermissionNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await PermissionSelectNames.Execute(Connection, PortalId));
    }
}