namespace Crudspa.Education.Publisher.Server.Services;

public class ActivityElementServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService) : IActivityElementService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ActivityTypeFull>>> FetchActivityTypes(Request request)
    {
        return await wrappers.Try<IList<ActivityTypeFull>>(request, async response =>
            await ActivityTypeSelectFull.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchContentAreaNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ContentAreaSelectNames.Execute(Connection));
    }
}