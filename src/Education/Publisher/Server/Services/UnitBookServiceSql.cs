namespace Crudspa.Education.Publisher.Server.Services;

public class UnitBookServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IUnitBookService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<UnitBook>>> FetchForUnit(Request<Unit> request)
    {
        return await wrappers.Try<IList<UnitBook>>(request, async response =>
        {
            var unitBooks = await UnitBookSelectForUnit.Execute(Connection, request.SessionId, request.Value.Id);
            return unitBooks;
        });
    }

    public async Task<Response<UnitBook?>> Fetch(Request<UnitBook> request)
    {
        return await wrappers.Try<UnitBook?>(request, async response =>
        {
            var unitBook = await UnitBookSelect.Execute(Connection, request.SessionId, request.Value);
            return unitBook;
        });
    }

    public async Task<Response<UnitBook?>> Add(Request<UnitBook> request)
    {
        return await wrappers.Validate<UnitBook?, UnitBook>(request, async response =>
        {
            var unitBook = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await UnitBookInsert.Execute(connection, transaction, request.SessionId, unitBook);

                return new UnitBook
                {
                    Id = id,
                    UnitId = unitBook.UnitId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<UnitBook> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var unitBook = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitBookUpdate.Execute(connection, transaction, request.SessionId, unitBook);
            });
        });
    }

    public async Task<Response> Remove(Request<UnitBook> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var unitBook = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitBookDelete.Execute(connection, transaction, request.SessionId, unitBook);
            });
        });
    }

    public async Task<Response<Copy>> Copy(Request<Copy> request)
    {
        return await wrappers.Validate<Copy, Copy>(request, async response =>
        {
            var unitBookRequest = new UnitBook
            {
                Id = request.Value.ExistingId,
            };

            var newUnitBook = await Fetch(new(request.SessionId, unitBookRequest));

            if (newUnitBook.Value is not null && newUnitBook.Ok)
            {
                newUnitBook.Value.Id = Guid.NewGuid();
                newUnitBook.Value.UnitId = request.Value.ExistingParentId;

                await Add(new(request.SessionId, newUnitBook.Value));
            }

            return new();
        });
    }

    public async Task<Response> SaveOrder(Request<IList<UnitBook>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var unitBooks = request.Value;

            unitBooks.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitBookUpdateOrdinals.Execute(connection, transaction, request.SessionId, unitBooks);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchBookNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await BookSelectNames.Execute(Connection));
    }
}