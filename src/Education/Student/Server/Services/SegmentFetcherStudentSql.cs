namespace Crudspa.Education.Student.Server.Services;

public class SegmentFetcherStudentSql(IServerConfigService configService) : ISegmentFetcher
{
    private String Connection => configService.Fetch().Database;

    public async Task<IList<NavSegment>> Fetch(Guid? sessionId, Guid? portalId)
    {
        return await SegmentSelectForSession.Execute(Connection, sessionId, portalId, "EducationStudent.SegmentSelectForSession");
    }
}