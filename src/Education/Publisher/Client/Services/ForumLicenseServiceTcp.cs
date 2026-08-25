namespace Crudspa.Education.Publisher.Client.Services;

using License = Shared.Contracts.Data.License;

public class ForumLicenseServiceTcp(IProxyWrappers proxyWrappers) : IForumLicenseService
{
    public async Task<Response<IList<ForumLicense>>> FetchForLicense(Request<License> request) =>
        await proxyWrappers.Send<IList<ForumLicense>>("ForumLicenseFetchForLicense", request);

    public async Task<Response<ForumLicense?>> Fetch(Request<ForumLicense> request) =>
        await proxyWrappers.Send<ForumLicense?>("ForumLicenseFetch", request);

    public async Task<Response<ForumLicense?>> Add(Request<ForumLicense> request) =>
        await proxyWrappers.Send<ForumLicense?>("ForumLicenseAdd", request);

    public async Task<Response> Save(Request<ForumLicense> request) =>
        await proxyWrappers.Send("ForumLicenseSave", request);

    public async Task<Response> Remove(Request<ForumLicense> request) =>
        await proxyWrappers.Send("ForumLicenseRemove", request);

    public async Task<Response<IList<Orderable>>> FetchForumNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("ForumLicenseFetchForumNames", request);
}