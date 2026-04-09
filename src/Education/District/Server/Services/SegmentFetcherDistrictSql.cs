namespace Crudspa.Education.District.Server.Services;

public class SegmentFetcherDistrictSql(IServerConfigService configService) : ISegmentFetcher
{
    private String Connection => configService.Fetch().Database;

    public async Task<IList<NavSegment>> Fetch(Guid? sessionId, Guid? portalId)
    {
        return await SegmentSelectForSession.Execute(Connection, sessionId, portalId, "EducationDistrict.SegmentSelectForSession");
    }
}