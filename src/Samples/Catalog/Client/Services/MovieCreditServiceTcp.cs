namespace Crudspa.Samples.Catalog.Client.Services;

public class MovieCreditServiceTcp(IProxyWrappers proxyWrappers) : IMovieCreditService
{
    public async Task<Response<IList<MovieCredit>>> FetchForMovie(Request<Movie> request) =>
        await proxyWrappers.Send<IList<MovieCredit>>("MovieCreditFetchForMovie", request);

    public async Task<Response<MovieCredit?>> Fetch(Request<MovieCredit> request) =>
        await proxyWrappers.Send<MovieCredit?>("MovieCreditFetch", request);

    public async Task<Response<MovieCredit?>> Add(Request<MovieCredit> request) =>
        await proxyWrappers.Send<MovieCredit?>("MovieCreditAdd", request);

    public async Task<Response> Save(Request<MovieCredit> request) =>
        await proxyWrappers.Send("MovieCreditSave", request);

    public async Task<Response> Remove(Request<MovieCredit> request) =>
        await proxyWrappers.Send("MovieCreditRemove", request);

    public async Task<Response> SaveOrder(Request<IList<MovieCredit>> request) =>
        await proxyWrappers.Send("MovieCreditSaveOrder", request);
}