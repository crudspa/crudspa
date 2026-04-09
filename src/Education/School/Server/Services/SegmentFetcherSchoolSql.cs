namespace Crudspa.Education.School.Server.Services;

public class SegmentFetcherSchoolSql(IServerConfigService configService) : ISegmentFetcher
{
    private String Connection => configService.Fetch().Database;

    public async Task<IList<NavSegment>> Fetch(Guid? sessionId, Guid? portalId)
    {
        return await SegmentSelectForSession.Execute(Connection, sessionId, portalId, "EducationSchool.SegmentSelectForSession");
    }
}