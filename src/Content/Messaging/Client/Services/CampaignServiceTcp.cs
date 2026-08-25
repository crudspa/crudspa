namespace Crudspa.Content.Messaging.Client.Services;

public class CampaignServiceTcp(IProxyWrappers proxyWrappers) : ICampaignService
{
    public async Task<Response<IList<Campaign>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<Campaign>>("CampaignFetchForPortal", request);

    public async Task<Response<Campaign?>> Fetch(Request<Campaign> request) =>
        await proxyWrappers.Send<Campaign?>("CampaignFetch", request);

    public async Task<Response<IList<Named>>> FetchLicenseNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("CampaignFetchLicenseNames", request);

    public async Task<Response<Campaign?>> Add(Request<Campaign> request) =>
        await proxyWrappers.Send<Campaign?>("CampaignAdd", request);

    public async Task<Response> Save(Request<Campaign> request) =>
        await proxyWrappers.Send("CampaignSave", request);

    public async Task<Response> Remove(Request<Campaign> request) =>
        await proxyWrappers.Send("CampaignRemove", request);
}