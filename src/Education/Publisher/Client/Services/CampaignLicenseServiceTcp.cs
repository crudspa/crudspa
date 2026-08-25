namespace Crudspa.Education.Publisher.Client.Services;

using License = Shared.Contracts.Data.License;

public class CampaignLicenseServiceTcp(IProxyWrappers proxyWrappers) : ICampaignLicenseService
{
    public async Task<Response<IList<CampaignLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<CampaignLicense>>("CampaignLicenseFetchForLicense", request);

    public async Task<Response<CampaignLicense?>> Fetch(Request<CampaignLicense> request) =>
        await proxyWrappers.Send<CampaignLicense?>("CampaignLicenseFetch", request);

    public async Task<Response<CampaignLicense?>> Add(Request<CampaignLicense> request) =>
        await proxyWrappers.Send<CampaignLicense?>("CampaignLicenseAdd", request);

    public async Task<Response> Save(Request<CampaignLicense> request) =>
        await proxyWrappers.Send("CampaignLicenseSave", request);

    public async Task<Response> Remove(Request<CampaignLicense> request) =>
        await proxyWrappers.Send("CampaignLicenseRemove", request);

    public async Task<Response<IList<Named>>> FetchCampaignNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("CampaignLicenseFetchCampaignNames", request);
}