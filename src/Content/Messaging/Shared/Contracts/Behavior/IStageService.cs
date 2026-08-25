namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IStageService
{
    Task<Response<IList<Stage>>> FetchForCampaign(Request<Campaign> request);
    Task<Response<Stage?>> Fetch(Request<Stage> request);
    Task<Response<Stage?>> Add(Request<Stage> request);
    Task<Response> Save(Request<Stage> request);
    Task<Response> Remove(Request<Stage> request);
    Task<Response> SaveOrder(Request<IList<Stage>> request);
}