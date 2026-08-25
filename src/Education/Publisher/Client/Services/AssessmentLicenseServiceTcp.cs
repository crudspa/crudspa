namespace Crudspa.Education.Publisher.Client.Services;

using License = Shared.Contracts.Data.License;

public class AssessmentLicenseServiceTcp(IProxyWrappers proxyWrappers) : IAssessmentLicenseService
{
    public async Task<Response<IList<AssessmentLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<AssessmentLicense>>("AssessmentLicenseFetchForLicense", request);

    public async Task<Response<AssessmentLicense?>> Fetch(Request<AssessmentLicense> request) =>
        await proxyWrappers.Send<AssessmentLicense?>("AssessmentLicenseFetch", request);

    public async Task<Response<AssessmentLicense?>> Add(Request<AssessmentLicense> request) =>
        await proxyWrappers.Send<AssessmentLicense?>("AssessmentLicenseAdd", request);

    public async Task<Response> Save(Request<AssessmentLicense> request) =>
        await proxyWrappers.Send("AssessmentLicenseSave", request);

    public async Task<Response> Remove(Request<AssessmentLicense> request) =>
        await proxyWrappers.Send("AssessmentLicenseRemove", request);

    public async Task<Response<IList<Named>>> FetchAssessmentNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("AssessmentLicenseFetchAssessmentNames", request);
}