namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

using License = Data.License;

public interface IForumLicenseService
{
    Task<Response<IList<ForumLicense>>> FetchForLicense(Request<License> request);
    Task<Response<ForumLicense?>> Fetch(Request<ForumLicense> request);
    Task<Response<ForumLicense?>> Add(Request<ForumLicense> request);
    Task<Response> Save(Request<ForumLicense> request);
    Task<Response> Remove(Request<ForumLicense> request);
    Task<Response<IList<Orderable>>> FetchForumNames(Request request);
}