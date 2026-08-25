namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IPopulationService
{
    Task<Response<PopulationRefreshResult?>> Refresh(Request<PopulationRefresh> request);
    Task<Response<IList<Population>>> FetchForPortal(Request<Portal> request);
    Task<Response<Population?>> Fetch(Request<Population> request);
    Task<Response<IList<PopulationToken>>> FetchTokens(Request<Population> request);
    Task<Response<Population?>> Add(Request<Population> request) =>
        Task.FromResult<Response<Population?>>(new() { Errors = [new() { Message = "Populations are developer-controlled lookup values." }] });
    Task<Response> Save(Request<Population> request) =>
        Task.FromResult<Response>(new() { Errors = [new() { Message = "Populations are developer-controlled lookup values." }] });
    Task<Response> Remove(Request<Population> request) =>
        Task.FromResult<Response>(new() { Errors = [new() { Message = "Populations are developer-controlled lookup values." }] });
}