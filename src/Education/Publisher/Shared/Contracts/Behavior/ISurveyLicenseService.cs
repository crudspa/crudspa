namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using License = Data.License;

public interface ISurveyLicenseService
{
    Task<Response<IList<SurveyLicense>>> FetchForLicense(Request<License> request);
    Task<Response<SurveyLicense?>> Fetch(Request<SurveyLicense> request);
    Task<Response<SurveyLicense?>> Add(Request<SurveyLicense> request);
    Task<Response> Save(Request<SurveyLicense> request);
    Task<Response> Remove(Request<SurveyLicense> request);
    Task<Response<IList<Named>>> FetchSurveyNames(Request request);
}