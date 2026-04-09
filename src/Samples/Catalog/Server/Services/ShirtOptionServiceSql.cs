namespace Crudspa.Samples.Catalog.Server.Services;

public class ShirtOptionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IShirtOptionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ShirtOption>>> FetchForShirt(Request<Shirt> request)
    {
        return await wrappers.Try<IList<ShirtOption>>(request, async response =>
        {
            var shirtOptions = await ShirtOptionSelectForShirt.Execute(Connection, request.SessionId, request.Value.Id);

            return shirtOptions;
        });
    }

    public async Task<Response<ShirtOption?>> Fetch(Request<ShirtOption> request)
    {
        return await wrappers.Try<ShirtOption?>(request, async response =>
        {
            var shirtOption = await ShirtOptionSelect.Execute(Connection, request.SessionId, request.Value);

            return shirtOption;
        });
    }

    public async Task<Response<ShirtOption?>> Add(Request<ShirtOption> request)
    {
        return await wrappers.Validate<ShirtOption?, ShirtOption>(request, async response =>
        {
            var shirtOption = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ShirtOptionInsert.Execute(connection, transaction, request.SessionId, shirtOption);

                return new ShirtOption
                {
                    Id = id,
                    ShirtId = shirtOption.ShirtId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ShirtOption> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var shirtOption = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ShirtOptionUpdate.Execute(connection, transaction, request.SessionId, shirtOption);
            });
        });
    }

    public async Task<Response> Remove(Request<ShirtOption> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var shirtOption = request.Value;
            var existing = await ShirtOptionSelect.Execute(Connection, request.SessionId, shirtOption);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ShirtOptionDelete.Execute(connection, transaction, request.SessionId, shirtOption);
            });
        });
    }

    public async Task<Response> SaveRelations(Request<ShirtOption> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var shirtOption = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ShirtOptionUpdateRelations.Execute(connection, transaction, request.SessionId, shirtOption);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<ShirtOption>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var shirtOptions = request.Value;

            shirtOptions.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ShirtOptionUpdateOrdinals.Execute(connection, transaction, request.SessionId, shirtOptions);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchColorNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ColorSelectOrderables.Execute(Connection, request.SessionId));
    }
}