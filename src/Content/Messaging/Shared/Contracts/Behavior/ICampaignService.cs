namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface ICampaignService
{
    Task<Response<IList<Campaign>>> FetchForPortal(Request<Portal> request);
    Task<Response<Campaign?>> Fetch(Request<Campaign> request);
    Task<Response<IList<Named>>> FetchLicenseNames(Request request);
    Task<Response<Campaign?>> Add(Request<Campaign> request);
    Task<Response> Save(Request<Campaign> request);
    Task<Response> Remove(Request<Campaign> request);
}