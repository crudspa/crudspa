namespace Crudspa.Samples.Catalog.Server.Services;

public class MovieServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IMovieService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Movie>>> Search(Request<MovieSearch> request)
    {
        return await wrappers.Try<IList<Movie>>(request, async response =>
        {
            var movies = await MovieSelectWhere.Execute(Connection, request.SessionId, request.Value);

            return movies;
        });
    }

    public async Task<Response<Movie?>> Fetch(Request<Movie> request)
    {
        return await wrappers.Try<Movie?>(request, async response =>
        {
            var movie = await MovieSelect.Execute(Connection, request.SessionId, request.Value);

            return movie;
        });
    }

    public async Task<Response<Movie?>> Add(Request<Movie> request)
    {
        return await wrappers.Validate<Movie?, Movie>(request, async response =>
        {
            var movie = request.Value;

            var posterImageFileResponse = await fileService.SaveImage(new(request.SessionId, movie.PosterImageFile));
                if (!posterImageFileResponse.Ok)
                {
                    response.AddErrors(posterImageFileResponse.Errors);
                    return null;
                }

                movie.PosterImageFile.Id = posterImageFileResponse.Value.Id;

            var trailerVideoFileResponse = await fileService.SaveVideo(new(request.SessionId, movie.TrailerVideoFile));
                if (!trailerVideoFileResponse.Ok)
                {
                    response.AddErrors(trailerVideoFileResponse.Errors);
                    return null;
                }

                movie.TrailerVideoFile.Id = trailerVideoFileResponse.Value.Id;

            movie.Summary = htmlSanitizer.Sanitize(movie.Summary);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await MovieInsert.Execute(connection, transaction, request.SessionId, movie);

                return new Movie
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Movie> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var movie = request.Value;

            var existing = await MovieSelect.Execute(Connection, request.SessionId, movie);

            var posterImageFileResponse = await fileService.SaveImage(new(request.SessionId, movie.PosterImageFile), existing?.PosterImageFile);
            if (!posterImageFileResponse.Ok)
            {
                response.AddErrors(posterImageFileResponse.Errors);
                return;
            }

            movie.PosterImageFile.Id = posterImageFileResponse.Value.Id;

            var trailerVideoFileResponse = await fileService.SaveVideo(new(request.SessionId, movie.TrailerVideoFile), existing?.TrailerVideoFile);
            if (!trailerVideoFileResponse.Ok)
            {
                response.AddErrors(trailerVideoFileResponse.Errors);
                return;
            }

            movie.TrailerVideoFile.Id = trailerVideoFileResponse.Value.Id;

            movie.Summary = htmlSanitizer.Sanitize(movie.Summary);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MovieUpdate.Execute(connection, transaction, request.SessionId, movie);
            });
        });
    }

    public async Task<Response> Remove(Request<Movie> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var movie = request.Value;
            var existing = await MovieSelect.Execute(Connection, request.SessionId, movie);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await MovieDelete.Execute(connection, transaction, request.SessionId, movie);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchGenreNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GenreSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchRatingNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await RatingSelectOrderables.Execute(Connection, request.SessionId));
    }
}