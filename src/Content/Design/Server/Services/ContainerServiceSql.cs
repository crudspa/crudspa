namespace Crudspa.Content.Design.Server.Services;

public class ContainerServiceSql(
    IServiceWrappers wrappers,
    IServerConfigService configService)
    : IContainerService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Orderable>>> FetchDirectionNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await DirectionSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchWrapNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await WrapSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchJustifyContentNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await JustifyContentSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchAlignItemsNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await AlignItemsSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchAlignContentNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await AlignContentSelectOrderables.Execute(Connection));
    }
}