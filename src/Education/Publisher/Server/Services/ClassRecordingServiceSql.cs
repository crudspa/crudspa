namespace Crudspa.Education.Publisher.Server.Services;

public class ClassRecordingServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : IClassRecordingService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ClassRecording>>> Search(Request<ClassRecordingSearch> request)
    {
        return await wrappers.Try<IList<ClassRecording>>(request, async response =>
        {
            return await ClassRecordingSelectWhere.Execute(Connection, request.SessionId, request.Value);
        });
    }
}