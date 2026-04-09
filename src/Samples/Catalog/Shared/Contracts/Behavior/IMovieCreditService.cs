namespace Crudspa.Samples.Catalog.Shared.Contracts.Behavior;

public interface IMovieCreditService
{
    Task<Response<IList<MovieCredit>>> FetchForMovie(Request<Movie> request);
    Task<Response<MovieCredit?>> Fetch(Request<MovieCredit> request);
    Task<Response<MovieCredit?>> Add(Request<MovieCredit> request);
    Task<Response> Save(Request<MovieCredit> request);
    Task<Response> Remove(Request<MovieCredit> request);
    Task<Response> SaveOrder(Request<IList<MovieCredit>> request);
}