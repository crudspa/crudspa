namespace Crudspa.Content.Display.Server.Services;

public class SurveyRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionLicenseResolver sessionLicenseResolver)
    : ISurveyRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Survey?>> Fetch(Request<Survey> request)
    {
        return await wrappers.Try<Survey?>(request, async response =>
        {
            if (!request.Value.Id.HasValue)
            {
                response.AddError("Survey is not configured.");
                return null;
            }

            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            if (!await SurveyIsAccessible.Execute(Connection, licenseIds, surveyId: request.Value.Id))
                return null;

            return await SurveyRunSelect.Execute(Connection, request.SessionId, request.Value);
        });
    }

    public async Task<Response> Complete(Request<SurveyReply> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            if (!await SurveyIsAccessible.Execute(Connection, licenseIds, surveyReplyId: request.Value.Id))
            {
                response.AddError("Survey is not accessible.");
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyReplyComplete.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }
}