namespace Crudspa.Content.Design.Server.Services;

public class PostServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IPagePartsService pagePartsService)
    : IPostService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Post>>> SearchForBlog(Request<PostSearch> request)
    {
        return await wrappers.Try<IList<Post>>(request, async response =>
        {
            return await PostSelectWhereForBlog.Execute(Connection, request.SessionId, request.Value);
        });
    }

    public async Task<Response<Post?>> Fetch(Request<Post> request)
    {
        return await wrappers.Try<Post?>(request, async response =>
        {
            var post = await PostSelect.Execute(Connection, request.SessionId, request.Value);
            return post;
        });
    }

    public async Task<Response<Post?>> FetchPageId(Request<Post> request)
    {
        return await wrappers.Try<Post?>(request, async response =>
            await PostSelectPageId.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<Post?>> Add(Request<Post> request)
    {
        return await wrappers.Validate<Post?, Post>(request, async response =>
        {
            var post = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await PostInsert.Execute(connection, transaction, request.SessionId, post);

                return new Post
                {
                    Id = id,
                    BlogId = post.BlogId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Post> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var post = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PostUpdate.Execute(connection, transaction, request.SessionId, post);
            });
        });
    }

    public async Task<Response> Remove(Request<Post> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var post = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PostDelete.Execute(connection, transaction, request.SessionId, post);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchPageTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await PageTypeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<Page?>> FetchPage(Request<PostPage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);
            if (pageId.HasNothing())
                throw new("Post page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, new() { Id = pageId });
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response> SavePage(Request<PostPage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);
            var page = request.Value.Page;

            if (page is null || pageId.HasNothing())
                throw new("Post page not found.");

            page.Id = pageId;

            var saveResponse = await pagePartsService.SavePage(request.SessionId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<PostSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);

            if (pageId.HasNothing())
                throw new("Post page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<PostSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Post section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<PostSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Post page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<PostSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Post section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<PostSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing())
                throw new("Post section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<PostSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var pageId = await FetchPageId(request.SessionId, request.Value.PostId);
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing())
                throw new("Post page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchPageId(Guid? sessionId, Guid? postId)
    {
        var post = await PostSelect.Execute(Connection, sessionId, new() { Id = postId });
        return post?.Page?.Id;
    }
}