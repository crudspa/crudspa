namespace Crudspa.Education.Publisher.Server.Services;

public class ForumLicenseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IForumLicenseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ForumLicense>>> FetchForLicense(Request<License> request)
    {
        return await wrappers.Try<IList<ForumLicense>>(request, async response =>
        {
            var forumLicenses = await ForumLicenseSelectForLicense.Execute(Connection, request.SessionId, request.Value.Id);

            return forumLicenses;
        });
    }

    public async Task<Response<ForumLicense?>> Fetch(Request<ForumLicense> request)
    {
        return await wrappers.Try<ForumLicense?>(request, async response =>
        {
            var forumLicense = await ForumLicenseSelect.Execute(Connection, request.SessionId, request.Value);

            return forumLicense;
        });
    }

    public async Task<Response<ForumLicense?>> Add(Request<ForumLicense> request)
    {
        return await wrappers.Validate<ForumLicense?, ForumLicense>(request, async response =>
        {
            var forumLicense = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ForumLicenseInsert.Execute(connection, transaction, request.SessionId, forumLicense);

                return new ForumLicense
                {
                    Id = id,
                    LicenseId = forumLicense.LicenseId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ForumLicense> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var forumLicense = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ForumLicenseUpdate.Execute(connection, transaction, request.SessionId, forumLicense);
            });
        });
    }

    public async Task<Response> Remove(Request<ForumLicense> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var forumLicense = request.Value;
            var existing = await ForumLicenseSelect.Execute(Connection, request.SessionId, forumLicense);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ForumLicenseDelete.Execute(connection, transaction, request.SessionId, forumLicense);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchForumNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await Crudspa.Education.Publisher.Server.Sproxies.ForumSelectOrderables.Execute(Connection, request.SessionId));
    }
}