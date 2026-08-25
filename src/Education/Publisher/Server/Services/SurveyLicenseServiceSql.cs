namespace Crudspa.Education.Publisher.Server.Services;

public class SurveyLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : ISurveyLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<SurveyLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<SurveyLicense>>(request, async response =>
        {
            var surveyLicenses = await SurveyLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return surveyLicenses;
        });
    }

    public async Task<Response<SurveyLicense?>> Fetch(Request<SurveyLicense> request)
    {
        return await wrappers.Try<SurveyLicense?>(request, async response =>
        {
            var surveyLicense = await SurveyLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return surveyLicense;
        });
    }

    public async Task<Response<SurveyLicense?>> Add(Request<SurveyLicense> request)
    {
        return await wrappers.Validate<SurveyLicense?, SurveyLicense>(request, async response =>
        {
            var surveyLicense = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await SurveyLicenseInsert.Execute(connection, transaction, request.SessionId, surveyLicense);

                return new SurveyLicense
                {
                    Id = id,
                    LicenseId = surveyLicense.LicenseId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<SurveyLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var surveyLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyLicenseUpdate.Execute(connection, transaction, request.SessionId, surveyLicense);
            });
        });
    }

    public async Task<Response> Remove(Request<SurveyLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var surveyLicense = request.Value;
            var existing = await SurveyLicenseSelect.Execute(Connection, request.SessionId, surveyLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyLicenseDelete.Execute(connection, transaction, request.SessionId, surveyLicense);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchSurveyNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await Crudspa.Education.Publisher.Server.Sproxies.SurveySelectNames.Execute(Connection, request.SessionId));
    }
}