namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using License = Data.License;

public interface ICampaignLicenseService
{
    Task<Response<IList<CampaignLicense>>> FetchForLicense(Request<License> request);
    Task<Response<CampaignLicense?>> Fetch(Request<CampaignLicense> request);
    Task<Response<CampaignLicense?>> Add(Request<CampaignLicense> request);
    Task<Response> Save(Request<CampaignLicense> request);
    Task<Response> Remove(Request<CampaignLicense> request);
    Task<Response<IList<Named>>> FetchCampaignNames(Request request);
}