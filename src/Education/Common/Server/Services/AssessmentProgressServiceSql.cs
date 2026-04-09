namespace Crudspa.Education.Common.Server.Services;

public class AssessmentProgressServiceSql(IServiceWrappers wrappers, IServerConfigService configService)
    : IAssessmentProgressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<Assessment?>> Fetch(Request<AssessmentAssignment> request)
    {
        return await wrappers.Try<Assessment?>(request, async response =>
            await AssessmentSelectProgress.Execute(Connection, request.Value.Id));
    }
}