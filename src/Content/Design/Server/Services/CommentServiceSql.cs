using Crudspa.Content.Display.Server.Contracts.Data;
using Post = Crudspa.Content.Display.Shared.Contracts.Data.Post;

using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;
using Comment = Crudspa.Content.Display.Shared.Contracts.Data.Comment;
namespace Crudspa.Content.Design.Server.Services;

public class CommentServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IAudioFileService audioFileService,
    IImageFileService imageFileService,
    IPdfFileService pdfFileService,
    IVideoFileService videoFileService,
    IBlobService blobService,
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
            var createdMediaFiles = new List<CommentMedia>();
            Guid? forumId = null;

            if (comment.ThreadId.HasValue)
            {
                var thread = await ThreadSelect.Execute(Connection, request.SessionId,
                    new Thread { Id = comment.ThreadId });
                if (thread?.ForumId is null)
                {
                    response.AddError("Thread was not found.");
                    return null;
                }

                forumId = thread.ForumId;
            }

            comment.Body = htmlSanitizer.Sanitize(comment.Body);

            var saveFilesResponse = await SaveMediaFiles(request.SessionId, forumId, comment.CommentMedias, [],
                createdMediaFiles);
            if (!saveFilesResponse.Ok)
            {
                response.AddErrors(saveFilesResponse.Errors);
                await FileCleanup().Cleanup(request.SessionId, createdMediaFiles);
                return null;
            }

            try
            {
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
            }
            catch
            {
                await FileCleanup().Cleanup(request.SessionId, createdMediaFiles);
                throw;
            }
        });
    }

    public async Task<Response> Save(Request<Comment> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var comment = request.Value;

            var existing = await CommentSelect.Execute(Connection, request.SessionId, comment);

            if (existing is null)
            {
                response.AddError("Comment was not found.");
                return;
            }

            var existingCommentMedias = existing.CommentMedias;
            var createdMediaFiles = new List<CommentMedia>();
            Guid? forumId = null;

            if (existing.ThreadId.HasValue)
            {
                var thread = await ThreadSelect.Execute(Connection, request.SessionId,
                    new Thread { Id = existing.ThreadId });
                if (thread?.ForumId is null)
                {
                    response.AddError("Thread was not found.");
                    return;
                }

                forumId = thread.ForumId;
            }

            comment.Body = htmlSanitizer.Sanitize(comment.Body);

            var saveFilesResponse = await SaveMediaFiles(request.SessionId, forumId, comment.CommentMedias,
                existingCommentMedias, createdMediaFiles);
            if (!saveFilesResponse.Ok)
            {
                response.AddErrors(saveFilesResponse.Errors);
                await FileCleanup().Cleanup(request.SessionId, createdMediaFiles);
                return;
            }

            foreach (var commentMedia in comment.CommentMedias)
                commentMedia.CommentId = existing.Id;

            try
            {
                await sqlWrappers.WithTransaction(async (connection, transaction) =>
                {
                    await CommentUpdate.Execute(connection, transaction, request.SessionId, comment);

                    await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                        existingCommentMedias,
                        comment.CommentMedias,
                        CommentMediaInsertByBatch.Execute,
                        CommentMediaUpdateByBatch.Execute,
                        CommentMediaDeleteByBatch.Execute);

                    comment.CommentMedias.EnsureOrder();
                    await CommentMediaUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, comment.CommentMedias);
                });
            }
            catch
            {
                await FileCleanup().Cleanup(request.SessionId, createdMediaFiles);
                throw;
            }

            await FileCleanup().Cleanup(request.SessionId,
                CommentMediaFileCleanup.Difference(existingCommentMedias, comment.CommentMedias));
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

            await FileCleanup().Cleanup(request.SessionId, existing.CommentMedias);
        });
    }

    private async Task<Response> SaveMediaFiles(Guid? sessionId, Guid? forumId, IEnumerable<CommentMedia> medias,
        IEnumerable<CommentMedia> existingMedias, IList<CommentMedia> createdMediaFiles)
    {
        var response = new Response();
        var existing = existingMedias.ToList();

        foreach (var media in medias)
        {
            var existingMedia = existing.FirstOrDefault(x => x.Id.Equals(media.Id));
            var usesExistingFile = existingMedia is not null
                                   && CommentMediaFileCleanup.SameFile(media, existingMedia)
                                   && CommentMediaFileCleanup.FileId(media).HasValue;

            if (!usesExistingFile && CommentMediaFileCleanup.FileId(media).HasValue)
            {
                response.AddError("New or replacement media must be uploaded before it can be saved.", nameof(CommentMedia.Type));
                return response;
            }

            if (!usesExistingFile && forumId.HasValue)
            {
                var staged = await ForumRunUploadStage.Consume(Connection, sessionId, forumId, media.Type,
                    CommentMediaFileCleanup.BlobId(media), []);

                if (staged is null)
                {
                    response.AddError("Forum media upload is invalid, expired, or belongs to another session.",
                        nameof(CommentMedia.Type));
                    return response;
                }

                ApplyStage(media, staged);

                if (!staged.BlobId.HasValue
                    || staged.Bytes is <= 0
                    || staged.Bytes > ForumMediaPolicy.MaxBytes(media.Type)
                    || !await blobService.Exists(staged.BlobId.Value))
                {
                    createdMediaFiles.Add(media);
                    response.AddError("Forum media upload is missing or outside the allowed size.",
                        nameof(CommentMedia.Type));
                    return response;
                }
            }

            if (!usesExistingFile)
                createdMediaFiles.Add(media);

            media.CommentId = existingMedia?.CommentId ?? media.CommentId;

            switch (media.Type)
            {
                case CommentMedia.Types.Audio:
                {
                    var result = await fileService.SaveAudio(new(sessionId, media.AudioFile),
                        usesExistingFile ? existingMedia!.AudioFile : null);
                    if (!ApplySavedFile(result, value => media.AudioFile = value, response))
                        return response;
                    break;
                }
                case CommentMedia.Types.Image:
                {
                    var result = await fileService.SaveImage(new(sessionId, media.ImageFile),
                        usesExistingFile ? existingMedia!.ImageFile : null);
                    if (!ApplySavedFile(result, value => media.ImageFile = value, response))
                        return response;
                    if (forumId.HasValue
                        && !usesExistingFile
                        && !ForumMediaPolicy.HasValidImageDimensions(media.ImageFile.Width, media.ImageFile.Height))
                    {
                        response.AddError("Image attachment is not a valid supported image or exceeds the dimension limit.",
                            nameof(CommentMedia.Type));
                        return response;
                    }
                    break;
                }
                case CommentMedia.Types.Pdf:
                {
                    var result = await fileService.SavePdf(new(sessionId, media.PdfFile),
                        usesExistingFile ? existingMedia!.PdfFile : null);
                    if (!ApplySavedFile(result, value => media.PdfFile = value, response))
                        return response;
                    break;
                }
                case CommentMedia.Types.Video:
                {
                    var result = await fileService.SaveVideo(new(sessionId, media.VideoFile),
                        usesExistingFile ? existingMedia!.VideoFile : null);
                    if (!ApplySavedFile(result, value => media.VideoFile = value, response))
                        return response;
                    break;
                }
            }

            ClearUnselectedFiles(media);
        }

        return response;
    }

    private static Boolean ApplySavedFile<TFile>(Response<TFile?> result, Action<TFile> apply, Response response)
        where TFile : class
    {
        if (!result.Ok)
        {
            response.AddErrors(result.Errors);
            return false;
        }

        if (result.Value is null)
        {
            response.AddError("Media file could not be saved.", nameof(CommentMedia.Type));
            return false;
        }

        apply(result.Value);
        return true;
    }

    private static void ClearUnselectedFiles(CommentMedia media)
    {
        if (media.Type != CommentMedia.Types.Audio) media.AudioFile = new();
        if (media.Type != CommentMedia.Types.Image) media.ImageFile = new();
        if (media.Type != CommentMedia.Types.Pdf) media.PdfFile = new();
        if (media.Type != CommentMedia.Types.Video) media.VideoFile = new();
    }

    private static void ApplyStage(CommentMedia media, ForumUploadStage staged)
    {
        switch (media.Type)
        {
            case CommentMedia.Types.Audio:
                media.AudioFile = new()
                {
                    BlobId = staged.BlobId,
                    Name = staged.Name,
                    Format = staged.Format,
                    OptimizedStatus = AudioFile.OptimizationStatus.None,
                };
                break;
            case CommentMedia.Types.Image:
                media.ImageFile = new()
                {
                    BlobId = staged.BlobId,
                    Name = staged.Name,
                    Format = staged.Format,
                    OptimizedStatus = ImageFile.OptimizationStatus.None,
                };
                break;
            case CommentMedia.Types.Pdf:
                media.PdfFile = new()
                {
                    BlobId = staged.BlobId,
                    Name = staged.Name,
                    Format = staged.Format,
                };
                break;
            case CommentMedia.Types.Video:
                media.VideoFile = new()
                {
                    BlobId = staged.BlobId,
                    Name = staged.Name,
                    Format = staged.Format,
                    OptimizedStatus = VideoFile.OptimizationStatus.None,
                };
                break;
        }

        ClearUnselectedFiles(media);
    }

    private CommentMediaFileCleanup FileCleanup() =>
        new(Connection, audioFileService, imageFileService, pdfFileService, videoFileService, blobService);
}