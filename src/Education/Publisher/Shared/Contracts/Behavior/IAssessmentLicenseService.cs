namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using License = Data.License;

public interface IAssessmentLicenseService
{
    Task<Response<IList<AssessmentLicense>>> FetchForLicense(Request<License> request);
    Task<Response<AssessmentLicense?>> Fetch(Request<AssessmentLicense> request);
    Task<Response<AssessmentLicense?>> Add(Request<AssessmentLicense> request);
    Task<Response> Save(Request<AssessmentLicense> request);
    Task<Response> Remove(Request<AssessmentLicense> request);
    Task<Response<IList<Named>>> FetchAssessmentNames(Request request);
}