namespace Crudspa.Education.Publisher.Client.Services;

using License = Shared.Contracts.Data.License;

public class BlogLicenseServiceTcp(IProxyWrappers proxyWrappers) : IBlogLicenseService
{
    public async Task<Response<IList<BlogLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<BlogLicense>>("BlogLicenseFetchForLicense", request);

    public async Task<Response<BlogLicense?>> Fetch(Request<BlogLicense> request) =>
        await proxyWrappers.Send<BlogLicense?>("BlogLicenseFetch", request);

    public async Task<Response<BlogLicense?>> Add(Request<BlogLicense> request) =>
        await proxyWrappers.Send<BlogLicense?>("BlogLicenseAdd", request);

    public async Task<Response> Save(Request<BlogLicense> request) =>
        await proxyWrappers.Send("BlogLicenseSave", request);

    public async Task<Response> Remove(Request<BlogLicense> request) =>
        await proxyWrappers.Send("BlogLicenseRemove", request);

    public async Task<Response<IList<Named>>> FetchBlogNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("BlogLicenseFetchBlogNames", request);
}