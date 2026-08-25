namespace Crudspa.Education.Publisher.Server.Services;

public class BlogLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IBlogLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<BlogLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<BlogLicense>>(request, async response =>
        {
            var blogLicenses = await BlogLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return blogLicenses;
        });
    }

    public async Task<Response<BlogLicense?>> Fetch(Request<BlogLicense> request)
    {
        return await wrappers.Try<BlogLicense?>(request, async response =>
        {
            var blogLicense = await BlogLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return blogLicense;
        });
    }

    public async Task<Response<BlogLicense?>> Add(Request<BlogLicense> request)
    {
        return await wrappers.Validate<BlogLicense?, BlogLicense>(request, async response =>
        {
            var blogLicense = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await BlogLicenseInsert.Execute(connection, transaction, request.SessionId, blogLicense);

                return new BlogLicense
                {
                    Id = id,
                    LicenseId = blogLicense.LicenseId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<BlogLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var blogLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BlogLicenseUpdate.Execute(connection, transaction, request.SessionId, blogLicense);
            });
        });
    }

    public async Task<Response> Remove(Request<BlogLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var blogLicense = request.Value;
            var existing = await BlogLicenseSelect.Execute(Connection, request.SessionId, blogLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await BlogLicenseDelete.Execute(connection, transaction, request.SessionId, blogLicense);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchBlogNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await Crudspa.Education.Publisher.Server.Sproxies.BlogSelectNames.Execute(Connection, request.SessionId));
    }
}