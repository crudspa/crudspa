namespace Crudspa.Education.Publisher.Server.Services;

public class AssessmentLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IAssessmentLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<AssessmentLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<AssessmentLicense>>(request, async response =>
        {
            var assessmentLicenses = await AssessmentLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return assessmentLicenses;
        });
    }

    public async Task<Response<AssessmentLicense?>> Fetch(Request<AssessmentLicense> request)
    {
        return await wrappers.Try<AssessmentLicense?>(request, async response =>
        {
            var assessmentLicense = await AssessmentLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return assessmentLicense;
        });
    }

    public async Task<Response<AssessmentLicense?>> Add(Request<AssessmentLicense> request)
    {
        return await wrappers.Validate<AssessmentLicense?, AssessmentLicense>(request, async response =>
        {
            var assessmentLicense = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await AssessmentLicenseInsert.Execute(connection, transaction, request.SessionId, assessmentLicense);

                return new AssessmentLicense
                {
                    Id = id,
                    LicenseId = assessmentLicense.LicenseId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<AssessmentLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var assessmentLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentLicenseUpdate.Execute(connection, transaction, request.SessionId, assessmentLicense);
            });
        });
    }

    public async Task<Response> Remove(Request<AssessmentLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var assessmentLicense = request.Value;
            var existing = await AssessmentLicenseSelect.Execute(Connection, request.SessionId, assessmentLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentLicenseDelete.Execute(connection, transaction, request.SessionId, assessmentLicense);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchAssessmentNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await Crudspa.Education.Publisher.Server.Sproxies.AssessmentSelectNames.Execute(Connection, request.SessionId));
    }
}