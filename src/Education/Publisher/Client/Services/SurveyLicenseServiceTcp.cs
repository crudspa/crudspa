namespace Crudspa.Education.Publisher.Client.Services;

using License = Shared.Contracts.Data.License;

public class SurveyLicenseServiceTcp(IProxyWrappers proxyWrappers) : ISurveyLicenseService
{
    public async Task<Response<IList<SurveyLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<SurveyLicense>>("SurveyLicenseFetchForLicense", request);

    public async Task<Response<SurveyLicense?>> Fetch(Request<SurveyLicense> request) =>
        await proxyWrappers.Send<SurveyLicense?>("SurveyLicenseFetch", request);

    public async Task<Response<SurveyLicense?>> Add(Request<SurveyLicense> request) =>
        await proxyWrappers.Send<SurveyLicense?>("SurveyLicenseAdd", request);

    public async Task<Response> Save(Request<SurveyLicense> request) =>
        await proxyWrappers.Send("SurveyLicenseSave", request);

    public async Task<Response> Remove(Request<SurveyLicense> request) =>
        await proxyWrappers.Send("SurveyLicenseRemove", request);

    public async Task<Response<IList<Named>>> FetchSurveyNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("SurveyLicenseFetchSurveyNames", request);
}