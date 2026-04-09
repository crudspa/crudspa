namespace Crudspa.Samples.Catalog.Server.Services;

public class MovieCreditServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IMovieCreditService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<MovieCredit>>> FetchForMovie(Request<Movie> request)
    {
        return await wrappers.Try<IList<MovieCredit>>(request, async response =>
        {
            var movieCredits = await MovieCreditSelectForMovie.Execute(Connection, request.SessionId, request.Value.Id);

            return movieCredits;
        });
    }

    public async Task<Response<MovieCredit?>> Fetch(Request<MovieCredit> request)
    {
        return await wrappers.Try<MovieCredit?>(request, async response =>
        {
            var movieCredit = await MovieCreditSelect.Execute(Connection, request.SessionId, request.Value);

            return movieCredit;
        });
    }

    public async Task<Response<MovieCredit?>> Add(Request<MovieCredit> request)
    {
        return await wrappers.Validate<MovieCredit?, MovieCredit>(request, async response =>
        {
            var movieCredit = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await MovieCreditInsert.Execute(connection, transaction, request.SessionId, movieCredit);

                return new MovieCredit
                {
                    Id = id,
                    MovieId = movieCredit.MovieId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<MovieCredit> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var movieCredit = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MovieCreditUpdate.Execute(connection, transaction, request.SessionId, movieCredit);
            });
        });
    }

    public async Task<Response> Remove(Request<MovieCredit> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var movieCredit = request.Value;
            var existing = await MovieCreditSelect.Execute(Connection, request.SessionId, movieCredit);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MovieCreditDelete.Execute(connection, transaction, request.SessionId, movieCredit);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<MovieCredit>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var movieCredits = request.Value;

            movieCredits.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MovieCreditUpdateOrdinals.Execute(connection, transaction, request.SessionId, movieCredits);
            });
        });
    }
}