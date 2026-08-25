namespace Crudspa.Content.Messaging.Client.Services;

public class PopulationServiceTcp(IProxyWrappers proxyWrappers) : IPopulationService
{
    public async Task<Response<PopulationRefreshResult?>> Refresh(Request<PopulationRefresh> request) =>
        await proxyWrappers.Send<PopulationRefreshResult?>("PopulationRefresh", request);

    public async Task<Response<IList<Population>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Population>>("PopulationFetchForPortal", request);

    public async Task<Response<Population?>> Fetch(Request<Population> request) =>
        await proxyWrappers.Send<Population?>("PopulationFetch", request);

    public async Task<Response<IList<PopulationToken>>> FetchTokens(Request<Population> request) =>
        await proxyWrappers.Send<IList<PopulationToken>>("PopulationFetchTokens", request);

    public async Task<Response<Population?>> Add(Request<Population> request) =>
        await proxyWrappers.Send<Population?>("PopulationAdd", request);

    public async Task<Response> Save(Request<Population> request) =>
        await proxyWrappers.Send("PopulationSave", request);

    public async Task<Response> Remove(Request<Population> request) =>
        await proxyWrappers.Send("PopulationRemove", request);
}