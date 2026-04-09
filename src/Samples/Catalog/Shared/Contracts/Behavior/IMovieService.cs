namespace Crudspa.Samples.Catalog.Shared.Contracts.Behavior;

public interface IMovieService
{
    Task<Response<IList<Movie>>> Search(Request<MovieSearch> request);
    Task<Response<Movie?>> Fetch(Request<Movie> request);
    Task<Response<Movie?>> Add(Request<Movie> request);
    Task<Response> Save(Request<Movie> request);
    Task<Response> Remove(Request<Movie> request);
    Task<Response<IList<Orderable>>> FetchGenreNames(Request request);
    Task<Response<IList<Orderable>>> FetchRatingNames(Request request);
}