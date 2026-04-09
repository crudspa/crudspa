using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;
using ForumDelete = Crudspa.Education.Publisher.Server.Sproxies.ForumDelete;
using ForumInsert = Crudspa.Education.Publisher.Server.Sproxies.ForumInsert;
using ForumSelect = Crudspa.Education.Publisher.Server.Sproxies.ForumSelect;
using ForumUpdate = Crudspa.Education.Publisher.Server.Sproxies.ForumUpdate;

namespace Crudspa.Education.Publisher.Server.Services;

public class ForumServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer)
    : Shared.Contracts.Behavior.IForumService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Forum>>> Search(Request<ForumSearch> request)
    {
        return await wrappers.Try<IList<Forum>>(request, async response =>
        {
            return await ForumSelectWhere.Execute(Connection, request.Value);
        });
    }

    public async Task<Response<Forum?>> Fetch(Request<Forum> request)
    {
        return await wrappers.Try<Forum?>(request, async response =>
        {
            var forum = await ForumSelect.Execute(Connection, request.Value);
            return forum;
        });
    }

    public async Task<Response<Forum?>> Add(Request<Forum> request)
    {
        return await wrappers.Validate<Forum?, Forum>(request, async response =>
        {
            var forum = request.Value;

            forum.Description = htmlSanitizer.Sanitize(forum.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ForumInsert.Execute(connection, transaction, request.SessionId, forum);

                return new Forum
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Forum> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var forum = request.Value;

            forum.Description = htmlSanitizer.Sanitize(forum.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ForumUpdate.Execute(connection, transaction, request.SessionId, forum);
            });
        });
    }

    public async Task<Response> Remove(Request<Forum> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var forum = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ForumDelete.Execute(connection, transaction, request.SessionId, forum);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchBodyTemplateNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BodyTemplateSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchDistrictNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await DistrictSelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchSchoolNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await SchoolSelectNames.Execute(Connection, request.SessionId));
    }
}