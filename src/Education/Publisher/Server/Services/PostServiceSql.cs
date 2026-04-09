using IPostService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IPostService;
using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;
using PostDelete = Crudspa.Education.Publisher.Server.Sproxies.PostDelete;
using PostInsert = Crudspa.Education.Publisher.Server.Sproxies.PostInsert;
using PostSearch = Crudspa.Education.Publisher.Shared.Contracts.Data.PostSearch;
using PostSelect = Crudspa.Education.Publisher.Server.Sproxies.PostSelect;
using PostUpdate = Crudspa.Education.Publisher.Server.Sproxies.PostUpdate;

namespace Crudspa.Education.Publisher.Server.Services;

public class PostServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IPostService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Post>>> SearchForForum(Request<PostSearch> request)
    {
        return await wrappers.Try<IList<Post>>(request, async response =>
        {
            return await PostSelectWhereForForum.Execute(Connection, request.Value);
        });
    }

    public async Task<Response<IList<Post>>> FetchTreeForParent(Request<Post> request)
    {
        return await wrappers.Try<IList<Post>>(request, async response =>
        {
            var posts = await PostSelectTreeForParent.Execute(Connection, request.SessionId, request.Value.Id);
            return posts;
        });
    }

    public async Task<Response<Post?>> Fetch(Request<Post> request)
    {
        return await wrappers.Try<Post?>(request, async response =>
        {
            var post = await PostSelect.Execute(Connection, request.SessionId, request.Value);

            if (post is not null)
                post.Name = $"Post by {post.ByFirstName} {post.ByLastName}";

            return post;
        });
    }

    public async Task<Response<Post?>> Add(Request<Post> request)
    {
        return await wrappers.Validate<Post?, Post>(request, async response =>
        {
            var post = request.Value;

            var audioFileResponse = await fileService.SaveAudio(new(request.SessionId, post.AudioFile));
            if (!audioFileResponse.Ok)
            {
                response.AddErrors(audioFileResponse.Errors);
                return null;
            }

            post.AudioFile.Id = audioFileResponse.Value.Id;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, post.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return null;
            }

            post.ImageFile.Id = imageFileResponse.Value.Id;

            var pdfFileResponse = await fileService.SavePdf(new(request.SessionId, post.PdfFile));
            if (!pdfFileResponse.Ok)
            {
                response.AddErrors(pdfFileResponse.Errors);
                return null;
            }

            post.PdfFile.Id = pdfFileResponse.Value.Id;

            var videoFileResponse = await fileService.SaveVideo(new(request.SessionId, post.VideoFile));
            if (!videoFileResponse.Ok)
            {
                response.AddErrors(videoFileResponse.Errors);
                return null;
            }

            post.VideoFile.Id = videoFileResponse.Value.Id;

            post.Body = htmlSanitizer.Sanitize(post.Body);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await PostInsert.Execute(connection, transaction, request.SessionId, post);

                return new Post
                {
                    Id = id,
                    ForumId = post.ForumId,
                    ParentId = post.ParentId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Post> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var post = request.Value;

            var existing = await PostSelect.Execute(Connection, request.SessionId, post);

            var audioFileResponse = await fileService.SaveAudio(new(request.SessionId, post.AudioFile), existing?.AudioFile);
            if (!audioFileResponse.Ok)
            {
                response.AddErrors(audioFileResponse.Errors);
                return;
            }

            post.AudioFile.Id = audioFileResponse.Value.Id;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, post.ImageFile), existing?.ImageFile);
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }

            post.ImageFile.Id = imageFileResponse.Value.Id;

            var pdfFileResponse = await fileService.SavePdf(new(request.SessionId, post.PdfFile), existing?.PdfFile);
            if (!pdfFileResponse.Ok)
            {
                response.AddErrors(pdfFileResponse.Errors);
                return;
            }

            post.PdfFile.Id = pdfFileResponse.Value.Id;

            var videoFileResponse = await fileService.SaveVideo(new(request.SessionId, post.VideoFile), existing?.VideoFile);
            if (!videoFileResponse.Ok)
            {
                response.AddErrors(videoFileResponse.Errors);
                return;
            }

            post.VideoFile.Id = videoFileResponse.Value.Id;

            post.Body = htmlSanitizer.Sanitize(post.Body);

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

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GradeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchClassroomTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ClassroomTypeSelectOrderables.Execute(Connection));
    }

    public async Task<Response> AddReaction(Request<PostReaction> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var postReaction = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await PostReactionInsert.Execute(connection, transaction, request.SessionId, postReaction);
            });
        });
    }
}