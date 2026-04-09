namespace Crudspa.Content.Design.Server.Services;

public class ItemServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : IItemService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Orderable>>> FetchBasisNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BasisSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchAlignSelfNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await AlignSelfSelectOrderables.Execute(Connection));
    }
}