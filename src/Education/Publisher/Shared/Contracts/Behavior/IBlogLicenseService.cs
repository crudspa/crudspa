namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using License = Data.License;

public interface IBlogLicenseService
{
    Task<Response<IList<BlogLicense>>> FetchForLicense(Request<License> request);
    Task<Response<BlogLicense?>> Fetch(Request<BlogLicense> request);
    Task<Response<BlogLicense?>> Add(Request<BlogLicense> request);
    Task<Response> Save(Request<BlogLicense> request);
    Task<Response> Remove(Request<BlogLicense> request);
    Task<Response<IList<Named>>> FetchBlogNames(Request request);
}