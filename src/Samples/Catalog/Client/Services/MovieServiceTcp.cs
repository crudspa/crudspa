namespace Crudspa.Samples.Catalog.Client.Services;

public class MovieServiceTcp(IProxyWrappers proxyWrappers) : IMovieService
{
    public async Task<Response<IList<Movie>>> Search(Request<MovieSearch> request) =>
        await proxyWrappers.Send<IList<Movie>>("MovieSearch", request);

    public async Task<Response<Movie?>> Fetch(Request<Movie> request) =>
        await proxyWrappers.Send<Movie?>("MovieFetch", request);

    public async Task<Response<Movie?>> Add(Request<Movie> request) =>
        await proxyWrappers.Send<Movie?>("MovieAdd", request);

    public async Task<Response> Save(Request<Movie> request) =>
        await proxyWrappers.Send("MovieSave", request);

    public async Task<Response> Remove(Request<Movie> request) =>
        await proxyWrappers.Send("MovieRemove", request);

    public async Task<Response<IList<Orderable>>> FetchGenreNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("MovieFetchGenreNames", request);

    public async Task<Response<IList<Orderable>>> FetchRatingNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("MovieFetchRatingNames", request);
}