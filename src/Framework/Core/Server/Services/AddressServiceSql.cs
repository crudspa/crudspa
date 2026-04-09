namespace Crudspa.Framework.Core.Server.Services;

public class AddressServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : IAddressService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Named>>> FetchUsaStateNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await UsaStateSelectNames.Execute(Connection));
    }
}