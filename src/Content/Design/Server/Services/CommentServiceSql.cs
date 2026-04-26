
using Post = Crudspa.Content.Display.Shared.Contracts.Data.Post;

using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Server.Services;

public class CommentServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : ICommentService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Comment>>> FetchTreeForPost(Request<Post> request)
    {
        return await wrappers.Try<IList<Comment>>(request, async response =>
        {
            var comments = await CommentSelectTreeForPost.Execute(Connection, request.SessionId, request.Value.Id);

            comments = comments.OrderBy(x => x.Posted).ToList();

            return Crudspa.Framework.Core.Shared.Extensions.TreeEx.BuildTree(comments, x => x.Id, x => x.ParentId, (x, children) => x.Children = children).ToList();
        });
    }

    public async Task<Response<IList<Comment>>> FetchTreeForThread(Request<Thread> request)
    {
        return await wrappers.Try<IList<Comment>>(request, async response =>
        {
            var comments = await CommentSelectTreeForThread.Execute(Connection, request.SessionId, request.Value.Id);

            comments = comments.OrderBy(x => x.Posted).ToList();

            return Crudspa.Framework.Core.Shared.Extensions.TreeEx.BuildTree(comments, x => x.Id, x => x.ParentId, (x, children) => x.Children = children).ToList();
        });
    }

    public async Task<Response<Comment?>> Fetch(Request<Comment> request)
    {
        return await wrappers.Try<Comment?>(request, async response =>
        {
            var comment = await CommentSelect.Execute(Connection, request.SessionId, request.Value);

            return comment;
        });
    }

    public async Task<Response<Comment?>> Add(Request<Comment> request)
    {
        return await wrappers.Validate<Comment?, Comment>(request, async response =>
        {
            var comment = request.Value;

            foreach (var commentMedia in comment.CommentMedias)
            {
                var commentMediaAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, commentMedia.AudioFile), commentMedia.AudioFile.Id);
                if (!commentMediaAudioFileResponse.Ok)
                {
                    response.AddErrors(commentMediaAudioFileResponse.Errors);
                    return null;
                }

                if (commentMediaAudioFileResponse.Value is not null) commentMedia.AudioFile = commentMediaAudioFileResponse.Value;
                var commentMediaImageFileResponse = await fileService.SaveImage(new(request.SessionId, commentMedia.ImageFile), commentMedia.ImageFile.Id);
                if (!commentMediaImageFileResponse.Ok)
                {
                    response.AddErrors(commentMediaImageFileResponse.Errors);
                    return null;
                }

                if (commentMediaImageFileResponse.Value is not null) commentMedia.ImageFile = commentMediaImageFileResponse.Value;
                var commentMediaPdfFileResponse = await fileService.SavePdf(new(request.SessionId, commentMedia.PdfFile), commentMedia.PdfFile.Id);
                if (!commentMediaPdfFileResponse.Ok)
                {
                    response.AddErrors(commentMediaPdfFileResponse.Errors);
                    return null;
                }

                if (commentMediaPdfFileResponse.Value is not null) commentMedia.PdfFile = commentMediaPdfFileResponse.Value;
                var commentMediaVideoFileResponse = await fileService.SaveVideo(new(request.SessionId, commentMedia.VideoFile), commentMedia.VideoFile.Id);
                if (!commentMediaVideoFileResponse.Ok)
                {
                    response.AddErrors(commentMediaVideoFileResponse.Errors);
                    return null;
                }

                if (commentMediaVideoFileResponse.Value is not null) commentMedia.VideoFile = commentMediaVideoFileResponse.Value;
            }

            comment.Body = htmlSanitizer.Sanitize(comment.Body);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await CommentInsert.Execute(connection, transaction, request.SessionId, comment);

                foreach (var commentMedia in comment.CommentMedias)
                {
                    commentMedia.CommentId = id;
                    await CommentMediaInsertByBatch.Execute(connection, transaction, request.SessionId, commentMedia);
                }

                return new Comment
                {
                    Id = id,
                    PostId = comment.PostId,
                    ThreadId = comment.ThreadId,
                    ParentId = comment.ParentId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Comment> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var comment = request.Value;

            var existing = await CommentSelect.Execute(Connection, request.SessionId, comment);

            foreach (var commentMedia in comment.CommentMedias)
            {
                var existingCommentMedia = existing?.CommentMedias.FirstOrDefault(x => x.Id.Equals(commentMedia.Id));

                if (commentMedia.AudioFile is not null)
                {
                    var commentMediaAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, commentMedia.AudioFile), existingCommentMedia?.AudioFile?.Id);
                    if (!commentMediaAudioFileResponse.Ok)
                    {
                        response.AddErrors(commentMediaAudioFileResponse.Errors);
                        return;
                    }

                    if (commentMediaAudioFileResponse.Value is not null) commentMedia.AudioFile = commentMediaAudioFileResponse.Value;
                }

                if (commentMedia.ImageFile is not null)
                {
                    var commentMediaImageFileResponse = await fileService.SaveImage(new(request.SessionId, commentMedia.ImageFile), existingCommentMedia?.ImageFile?.Id);
                    if (!commentMediaImageFileResponse.Ok)
                    {
                        response.AddErrors(commentMediaImageFileResponse.Errors);
                        return;
                    }

                    if (commentMediaImageFileResponse.Value is not null) commentMedia.ImageFile = commentMediaImageFileResponse.Value;
                }

                if (commentMedia.PdfFile is not null)
                {
                    var commentMediaPdfFileResponse = await fileService.SavePdf(new(request.SessionId, commentMedia.PdfFile), existingCommentMedia?.PdfFile?.Id);
                    if (!commentMediaPdfFileResponse.Ok)
                    {
                        response.AddErrors(commentMediaPdfFileResponse.Errors);
                        return;
                    }

                    if (commentMediaPdfFileResponse.Value is not null) commentMedia.PdfFile = commentMediaPdfFileResponse.Value;
                }

                if (commentMedia.VideoFile is not null)
                {
                    var commentMediaVideoFileResponse = await fileService.SaveVideo(new(request.SessionId, commentMedia.VideoFile), existingCommentMedia?.VideoFile?.Id);
                    if (!commentMediaVideoFileResponse.Ok)
                    {
                        response.AddErrors(commentMediaVideoFileResponse.Errors);
                        return;
                    }

                    if (commentMediaVideoFileResponse.Value is not null) commentMedia.VideoFile = commentMediaVideoFileResponse.Value;
                }
            }

            comment.Body = htmlSanitizer.Sanitize(comment.Body);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await CommentUpdate.Execute(connection, transaction, request.SessionId, comment);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.CommentMedias,
                    comment.CommentMedias,
                    CommentMediaInsertByBatch.Execute,
                    CommentMediaUpdateByBatch.Execute,
                    CommentMediaDeleteByBatch.Execute);

                comment.CommentMedias.EnsureOrder();
                await CommentMediaUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, comment.CommentMedias);
            });
        });
    }

    public async Task<Response> Remove(Request<Comment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var comment = request.Value;
            var existing = await CommentSelect.Execute(Connection, request.SessionId, comment);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                foreach (var commentMedia in existing.CommentMedias)
                    await CommentMediaDeleteByBatch.Execute(connection, transaction, request.SessionId, commentMedia);

                await CommentDelete.Execute(connection, transaction, request.SessionId, comment);
            });
        });
    }
}