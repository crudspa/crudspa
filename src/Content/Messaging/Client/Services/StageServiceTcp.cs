namespace Crudspa.Content.Messaging.Client.Services;

public class StageServiceTcp(IProxyWrappers proxyWrappers) : IStageService
{
    public async Task<Response<IList<Stage>>> FetchForCampaign(Request<Campaign> request) =>
        await proxyWrappers.Send<IList<Stage>>("StageFetchForCampaign", request);

    public async Task<Response<Stage?>> Fetch(Request<Stage> request) =>
        await proxyWrappers.Send<Stage?>("StageFetch", request);

    public async Task<Response<Stage?>> Add(Request<Stage> request) =>
        await proxyWrappers.Send<Stage?>("StageAdd", request);

    public async Task<Response> Save(Request<Stage> request) =>
        await proxyWrappers.Send("StageSave", request);

    public async Task<Response> Remove(Request<Stage> request) =>
        await proxyWrappers.Send("StageRemove", request);

    public async Task<Response> SaveOrder(Request<IList<Stage>> request) =>
        await proxyWrappers.Send("StageSaveOrder", request);
}