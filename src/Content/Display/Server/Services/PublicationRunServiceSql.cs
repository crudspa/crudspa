namespace Crudspa.Content.Display.Server.Services;

public class PublicationRunServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : IPublicationRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<PublicationSummary>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<PublicationSummary>>(request, async response =>
        {
            var publications = await PublicationSelectAll.Execute(Connection, request.SessionId);

            return publications;
        });
    }

    public async Task<Response<IList<Orderable>>> FetchPublicationResourceTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await Crudspa.Content.Display.Server.Sproxies.PublicationResourceTypeSelectOrderables.Execute(Connection, request.SessionId));
    }
}