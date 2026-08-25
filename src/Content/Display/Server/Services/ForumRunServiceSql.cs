using Crudspa.Content.Display.Server;
using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

namespace Crudspa.Content.Display.Server.Services;

public class ForumRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISessionLicenseResolver sessionLicenseResolver,
    IFileService fileService,
    IAudioFileService audioFileService,
    IImageFileService imageFileService,
    IPdfFileService pdfFileService,
    IVideoFileService videoFileService,
    IBlobService blobService,
    IForumMediaService forumMediaService,
    IHtmlSanitizer htmlSanitizer)
    : IForumRunService
{
    private static readonly HashSet<String> AllowedReactions =
    [
        "❤️", "👍", "👎", "👏", "🙌", "🤝", "💪", "🙏", "💡", "✅",
        "❓", "❗", "👀", "🤔", "🙂", "😊", "😄", "😂", "😮", "😢",
        "😕", "😔", "😠", "⭐", "✨", "🎉", "🚀", "📚", "🧠", "🏆",
    ];

    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Forum>>> FetchForums(Request request)
    {
        return await wrappers.Try<IList<Forum>>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await ForumRunForumSelect.ExecuteAll(Connection, request.SessionId, licenseIds);
        });
    }

    public async Task<Response<Forum?>> FetchForum(Request<Forum> request)
    {
        return await wrappers.Try<Forum?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await ForumRunForumSelect.Execute(Connection, request.SessionId, request.Value.Id, licenseIds);
        });
    }

    public async Task<Response<IList<Thread>>> SearchThreads(Request<ThreadSearch> request)
    {
        return await wrappers.Try<IList<Thread>>(request, async response =>
        {
            request.Value.Paged.PageNumber = Math.Max(1, request.Value.Paged.PageNumber);
            request.Value.Paged.PageSize = request.Value.Paged.PageSize <= 0
                ? 25
                : Math.Min(50, request.Value.Paged.PageSize);

            if (request.Value.Sort.Field is not ("Title" or "Posted" or "Activity"))
                request.Value.Sort.Field = "Activity";

            request.Value.Sort.Ascending ??= false;

            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await ForumRunThreadSelect.ExecuteSearch(Connection, request.SessionId, request.Value, licenseIds);
        });
    }

    public async Task<Response<Thread?>> FetchThread(Request<Thread> request)
    {
        return await wrappers.Try<Thread?>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            return await ForumRunThreadSelect.Execute(Connection, request.SessionId, request.Value.Id, licenseIds);
        });
    }

    public async Task<Response<Thread?>> AddThread(Request<Thread> request)
    {
        if (request.Value.Comment.Body?.Length > ForumPolicy.MaxBodyCharacters)
            return new($"Body cannot be longer than {ForumPolicy.MaxBodyCharacters:N0} characters.");

        request.Value.Comment.Body = htmlSanitizer.Sanitize(request.Value.Comment.Body);

        return await wrappers.Validate<Thread?, Thread>(request, async response =>
        {
            var thread = request.Value;
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var forum = await ForumRunForumSelect.Execute(Connection, request.SessionId, thread.ForumId, licenseIds);

            if (forum is null)
                throw new InvalidOperationException("Forum was not found or is not accessible.");

            var createdMedia = new List<CommentMedia>();
            var mediaErrors = await SaveMediaFiles(request.SessionId, thread.ForumId, thread.Comment.CommentMedias, [], createdMedia);

            if (mediaErrors.HasItems())
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                response.AddErrors(mediaErrors);
                return null;
            }

            try
            {
                return await sqlWrappers.WithTransaction(async (connection, transaction) =>
                {
                    var ids = await ForumRunThreadWrite.Insert(connection, transaction, request.SessionId, thread, licenseIds);

                    thread.Comment.Id = ids.CommentId;
                    await MergeMedia(connection, transaction, request.SessionId, thread.Comment, [], licenseIds);

                    return new Thread
                    {
                        Id = ids.ThreadId,
                        ForumId = thread.ForumId,
                        Comment = new() { Id = ids.CommentId },
                    };
                });
            }
            catch
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                throw;
            }
        });
    }

    public async Task<Response> SaveThread(Request<Thread> request)
    {
        if (request.Value.Comment.Body?.Length > ForumPolicy.MaxBodyCharacters)
            return new($"Body cannot be longer than {ForumPolicy.MaxBodyCharacters:N0} characters.");

        request.Value.Comment.Body = htmlSanitizer.Sanitize(request.Value.Comment.Body);

        return await wrappers.Validate(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var existing = await ForumRunThreadSelect.Execute(Connection, request.SessionId, request.Value.Id, licenseIds);

            if (existing is null)
                throw new InvalidOperationException("Thread was not found or is not accessible.");

            if (!existing.CanEdit)
                throw new InvalidOperationException("Thread cannot be edited by the current user.");

            request.Value.Comment.Id = existing.Comment.Id;
            var createdMedia = new List<CommentMedia>();
            var mediaErrors = await SaveMediaFiles(request.SessionId, existing.ForumId, request.Value.Comment.CommentMedias,
                existing.Comment.CommentMedias, createdMedia);

            if (mediaErrors.HasItems())
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                response.AddErrors(mediaErrors);
                return;
            }

            try
            {
                await sqlWrappers.WithTransaction(async (connection, transaction) =>
                {
                    await ForumRunThreadWrite.Update(connection, transaction, request.SessionId, request.Value, licenseIds);
                    await MergeMedia(connection, transaction, request.SessionId, request.Value.Comment, existing.Comment.CommentMedias, licenseIds);
                });
            }
            catch
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                throw;
            }

            await CleanupMediaFiles(request.SessionId,
                MediaDifference(existing.Comment.CommentMedias, request.Value.Comment.CommentMedias));
        });
    }

    public async Task<Response> RemoveThread(Request<Thread> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var existing = await ForumRunThreadSelect.Execute(Connection, request.SessionId, request.Value.Id, licenseIds)
                           ?? throw new InvalidOperationException("Thread was not found or is not accessible.");

            if (!existing.CanDelete)
                throw new InvalidOperationException("Thread cannot be deleted by the current user.");

            var comments = await ForumRunCommentSelect.Execute(Connection, request.SessionId, request.Value.Id, licenseIds);
            var media = existing.Comment.CommentMedias.Concat(comments.SelectMany(x => x.CommentMedias)).ToList();

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
                await ForumRunThreadWrite.Delete(connection, transaction, request.SessionId, request.Value.Id, licenseIds));

            await CleanupMediaFiles(request.SessionId, media);
        });
    }

    public async Task<Response<IList<Comment>>> FetchComments(Request<Thread> request)
    {
        return await wrappers.Try<IList<Comment>>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var comments = await ForumRunCommentSelect.Execute(Connection, request.SessionId, request.Value.Id, licenseIds);

            var tree = TreeEx.BuildTree(comments.OrderBy(x => x.Posted).ToList(),
                    x => x.Id, x => x.ParentId, (comment, children) => comment.Children = children)
                .ToList();

            foreach (var comment in tree)
                ApplyReplyDepth(comment, 1);

            return tree;
        });
    }

    private static void ApplyReplyDepth(Comment comment, Int32 depth)
    {
        if (depth >= ForumPolicy.MaxReplyDepth)
            comment.CanReply = false;

        foreach (var child in comment.Children)
            ApplyReplyDepth(child, depth + 1);
    }

    public async Task<Response<Comment?>> AddComment(Request<Comment> request)
    {
        if (request.Value.Body?.Length > ForumPolicy.MaxBodyCharacters)
            return new($"Body cannot be longer than {ForumPolicy.MaxBodyCharacters:N0} characters.");

        request.Value.Body = htmlSanitizer.Sanitize(request.Value.Body);

        return await wrappers.Validate<Comment?, Comment>(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var thread = await ForumRunThreadSelect.Execute(Connection, request.SessionId, request.Value.ThreadId, licenseIds);

            if (thread is null)
                throw new InvalidOperationException("Thread was not found or is not accessible.");

            if (request.Value.ParentId.HasValue)
            {
                var comments = await ForumRunCommentSelect.Execute(Connection, request.SessionId, request.Value.ThreadId, licenseIds);
                var parent = comments.FirstOrDefault(x => x.Id == request.Value.ParentId);

                if (parent is null || parent.Removed)
                    throw new InvalidOperationException("Reply parent is not in this thread.");

                var ancestorId = parent.Id;
                var parentDepth = 0;
                var visited = new HashSet<Guid?>();

                while (ancestorId.HasValue)
                {
                    if (!visited.Add(ancestorId))
                        throw new InvalidOperationException("The reply hierarchy is invalid.");

                    parentDepth++;
                    if (parentDepth >= ForumPolicy.MaxReplyDepth)
                        throw new InvalidOperationException($"Replies can be nested at most {ForumPolicy.MaxReplyDepth} levels deep.");

                    ancestorId = comments.FirstOrDefault(x => x.Id == ancestorId)?.ParentId;
                }
            }

            var createdMedia = new List<CommentMedia>();
            var mediaErrors = await SaveMediaFiles(request.SessionId, thread.ForumId, request.Value.CommentMedias, [], createdMedia);

            if (mediaErrors.HasItems())
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                response.AddErrors(mediaErrors);
                return null;
            }

            try
            {
                return await sqlWrappers.WithTransaction(async (connection, transaction) =>
                {
                    var id = await ForumRunCommentWrite.Insert(connection, transaction, request.SessionId, request.Value, licenseIds);
                    request.Value.Id = id;
                    await MergeMedia(connection, transaction, request.SessionId, request.Value, [], licenseIds);

                    return new Comment
                    {
                        Id = id,
                        ThreadId = request.Value.ThreadId,
                        ParentId = request.Value.ParentId,
                    };
                });
            }
            catch
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                throw;
            }
        });
    }

    public async Task<Response> SaveComment(Request<Comment> request)
    {
        if (request.Value.Body?.Length > ForumPolicy.MaxBodyCharacters)
            return new($"Body cannot be longer than {ForumPolicy.MaxBodyCharacters:N0} characters.");

        request.Value.Body = htmlSanitizer.Sanitize(request.Value.Body);

        return await wrappers.Validate(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var thread = await ForumRunThreadSelect.Execute(Connection, request.SessionId, request.Value.ThreadId, licenseIds)
                         ?? throw new InvalidOperationException("Thread was not found or is not accessible.");
            var comments = await ForumRunCommentSelect.Execute(Connection, request.SessionId, request.Value.ThreadId, licenseIds);
            var existing = comments.FirstOrDefault(x => x.Id == request.Value.Id)
                           ?? throw new InvalidOperationException("Comment was not found or is not accessible.");

            if (!existing.CanEdit)
                throw new InvalidOperationException("Comment cannot be edited by the current user.");

            var createdMedia = new List<CommentMedia>();
            var mediaErrors = await SaveMediaFiles(request.SessionId, thread.ForumId, request.Value.CommentMedias,
                existing.CommentMedias, createdMedia);

            if (mediaErrors.HasItems())
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                response.AddErrors(mediaErrors);
                return;
            }

            try
            {
                await sqlWrappers.WithTransaction(async (connection, transaction) =>
                {
                    await ForumRunCommentWrite.Update(connection, transaction, request.SessionId, request.Value, licenseIds);
                    await MergeMedia(connection, transaction, request.SessionId, request.Value, existing.CommentMedias, licenseIds);
                });
            }
            catch
            {
                await CleanupMediaFiles(request.SessionId, createdMedia);
                throw;
            }

            await CleanupMediaFiles(request.SessionId, MediaDifference(existing.CommentMedias, request.Value.CommentMedias));
        });
    }

    public async Task<Response> RemoveComment(Request<Comment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            var comments = await ForumRunCommentSelect.Execute(Connection, request.SessionId, request.Value.ThreadId, licenseIds);
            var existing = comments.FirstOrDefault(x => x.Id == request.Value.Id)
                           ?? throw new InvalidOperationException("Comment was not found or is not accessible.");

            if (!existing.CanDelete)
                throw new InvalidOperationException("Comment cannot be deleted by the current user.");

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
                await ForumRunCommentWrite.Delete(connection, transaction, request.SessionId, request.Value.Id, licenseIds));

            await CleanupMediaFiles(request.SessionId, existing.CommentMedias);
        });
    }

    public async Task<Response> SetReaction(Request<CommentReaction> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            if (request.Value.Emoji is not null && !AllowedReactions.Contains(request.Value.Emoji))
            {
                response.AddError("Reaction is invalid.", nameof(CommentReaction.Emoji));
                return;
            }

            var licenseIds = await sessionLicenseResolver.Fetch(request.SessionId);
            await ForumRunReactionSet.Execute(Connection, request.SessionId, request.Value, licenseIds);
        });
    }

    private async Task<List<Error>> SaveMediaFiles(Guid? sessionId, Guid? forumId, IEnumerable<CommentMedia> medias,
        IEnumerable<CommentMedia> existingMedias, ICollection<CommentMedia> createdMedias)
    {
        var errors = new List<Error>();
        var mediaList = medias.ToList();

        if (mediaList.Where(x => x.Id.HasValue).GroupBy(x => x.Id).Any(x => x.Count() > 1))
            errors.Add(new() { Field = nameof(Comment.CommentMedias), Message = "An attachment cannot be submitted more than once." });

        if (mediaList.Select(BlobId).Where(x => x.HasValue).GroupBy(x => x).Any(x => x.Count() > 1))
            errors.Add(new() { Field = nameof(Comment.CommentMedias), Message = "An uploaded file cannot be submitted more than once." });

        if (errors.HasItems())
            return errors;

        foreach (var media in mediaList)
        {
            var existing = existingMedias.FirstOrDefault(x => x.Id == media.Id);
            if (!ForumRunMediaProjection.TryHydrateUnchanged(media, existing))
            {
                errors.Add(new()
                {
                    Field = nameof(CommentMedia.Type),
                    Message = "Attachment does not belong to this comment.",
                });
                continue;
            }

            var selectedFileId = media.Type switch
            {
                CommentMedia.Types.Audio => media.AudioFile.Id,
                CommentMedia.Types.Image => media.ImageFile.Id,
                CommentMedia.Types.Pdf => media.PdfFile.Id,
                CommentMedia.Types.Video => media.VideoFile.Id,
                _ => null,
            };
            var existingFileId = existing is null || existing.Type != media.Type
                ? null
                : media.Type switch
                {
                    CommentMedia.Types.Audio => existing.AudioFile.Id,
                    CommentMedia.Types.Image => existing.ImageFile.Id,
                    CommentMedia.Types.Pdf => existing.PdfFile.Id,
                    CommentMedia.Types.Video => existing.VideoFile.Id,
                    _ => null,
                };

            if (selectedFileId.HasValue && (!existingFileId.HasValue || selectedFileId != existingFileId))
            {
                errors.Add(new()
                {
                    Field = nameof(CommentMedia.Type),
                    Message = "Attachment file does not belong to this comment.",
                });
                continue;
            }

            var selectedBlobId = BlobId(media);
            var existingBlobId = existing is null || existing.Type != media.Type ? null : BlobId(existing);
            var isUnchanged = existing is not null
                              && existing.Type == media.Type
                              && selectedFileId == existingFileId
                              && selectedBlobId == existingBlobId;

            if (!isUnchanged)
            {
                var staged = selectedBlobId.HasValue
                    ? await forumMediaService.Consume(sessionId, forumId, media.Type, selectedBlobId)
                    : null;

                if (staged is null)
                {
                    errors.Add(new()
                    {
                        Field = nameof(CommentMedia.Type),
                        Message = "Attachment upload is invalid, expired, already used, or belongs to another forum session.",
                    });
                    continue;
                }

                ApplyStage(media, staged);
                createdMedias.Add(media);
            }

            switch (media.Type)
            {
                case CommentMedia.Types.Audio:
                {
                    var existingId = media.AudioFile.Id == existing?.AudioFile.Id
                                     && media.AudioFile.BlobId == existing?.AudioFile.BlobId
                        ? existing?.AudioFile.Id
                        : null;
                    var result = await fileService.SaveAudio(new(sessionId, media.AudioFile), existingId);
                    if (!result.Ok) errors.AddRange(result.Errors);
                    else if (result.Value is not null) media.AudioFile = result.Value;
                    media.ImageFile = new();
                    media.PdfFile = new();
                    media.VideoFile = new();
                    break;
                }
                case CommentMedia.Types.Image:
                {
                    var existingId = media.ImageFile.Id == existing?.ImageFile.Id
                                     && media.ImageFile.BlobId == existing?.ImageFile.BlobId
                        ? existing?.ImageFile.Id
                        : null;
                    var result = await fileService.SaveImage(new(sessionId, media.ImageFile), existingId);
                    if (!result.Ok)
                        errors.AddRange(result.Errors);
                    else if (result.Value is null)
                        errors.Add(new()
                        {
                            Field = nameof(Comment.CommentMedias),
                            Message = "Image attachment could not be saved.",
                        });
                    else
                    {
                        media.ImageFile = result.Value;

                        if (!isUnchanged
                            && !ForumMediaPolicy.HasValidImageDimensions(result.Value.Width, result.Value.Height))
                            errors.Add(new()
                            {
                                Field = nameof(Comment.CommentMedias),
                                Message = "Image attachment is not a valid supported image or exceeds the dimension limit.",
                            });
                    }
                    media.AudioFile = new();
                    media.PdfFile = new();
                    media.VideoFile = new();
                    break;
                }
                case CommentMedia.Types.Pdf:
                {
                    var existingId = media.PdfFile.Id == existing?.PdfFile.Id
                                     && media.PdfFile.BlobId == existing?.PdfFile.BlobId
                        ? existing?.PdfFile.Id
                        : null;
                    var result = await fileService.SavePdf(new(sessionId, media.PdfFile), existingId);
                    if (!result.Ok) errors.AddRange(result.Errors);
                    else if (result.Value is not null) media.PdfFile = result.Value;
                    media.AudioFile = new();
                    media.ImageFile = new();
                    media.VideoFile = new();
                    break;
                }
                case CommentMedia.Types.Video:
                {
                    var existingId = media.VideoFile.Id == existing?.VideoFile.Id
                                     && media.VideoFile.BlobId == existing?.VideoFile.BlobId
                        ? existing?.VideoFile.Id
                        : null;
                    var result = await fileService.SaveVideo(new(sessionId, media.VideoFile), existingId);
                    if (!result.Ok) errors.AddRange(result.Errors);
                    else if (result.Value is not null) media.VideoFile = result.Value;
                    media.AudioFile = new();
                    media.ImageFile = new();
                    media.PdfFile = new();
                    break;
                }
            }
        }

        return errors;
    }

    private static Guid? BlobId(CommentMedia media) => media.Type switch
    {
        CommentMedia.Types.Audio => media.AudioFile.BlobId,
        CommentMedia.Types.Image => media.ImageFile.BlobId,
        CommentMedia.Types.Pdf => media.PdfFile.BlobId,
        CommentMedia.Types.Video => media.VideoFile.BlobId,
        _ => null,
    };

    private static Guid? FileId(CommentMedia media) => media.Type switch
    {
        CommentMedia.Types.Audio => media.AudioFile.Id,
        CommentMedia.Types.Image => media.ImageFile.Id,
        CommentMedia.Types.Pdf => media.PdfFile.Id,
        CommentMedia.Types.Video => media.VideoFile.Id,
        _ => null,
    };

    private static IList<CommentMedia> MediaDifference(IEnumerable<CommentMedia> source,
        IEnumerable<CommentMedia> other)
    {
        var compared = other.ToList();
        return source.Where(media => compared.All(item => !SameFile(media, item))).ToList();
    }

    private static Boolean SameFile(CommentMedia left, CommentMedia right) =>
        left.Type == right.Type
        && FileId(left) == FileId(right)
        && BlobId(left) == BlobId(right);

    private async Task CleanupMediaFiles(Guid? sessionId, IEnumerable<CommentMedia> medias)
    {
        foreach (var media in medias
                     .Where(x => FileId(x).HasValue || BlobId(x).HasValue)
                     .DistinctBy(x => (x.Type, FileId(x), BlobId(x))))
        {
            var fileId = FileId(media);
            if (fileId.HasValue)
            {
                if (await ForumRunCommentMediaFileReference.Exists(Connection, media.Type, fileId.Value))
                    continue;

                switch (media.Type)
                {
                    case CommentMedia.Types.Audio:
                        await audioFileService.Remove(new(sessionId, new AudioFile { Id = fileId }));
                        break;
                    case CommentMedia.Types.Image:
                        await RemoveImageFile(sessionId, fileId);
                        break;
                    case CommentMedia.Types.Pdf:
                        await pdfFileService.Remove(new(sessionId, new PdfFile { Id = fileId }));
                        break;
                    case CommentMedia.Types.Video:
                        await videoFileService.Remove(new(sessionId, new VideoFile { Id = fileId }));
                        break;
                }

                continue;
            }

            var blobId = BlobId(media);
            if (blobId.HasValue)
                await blobService.Remove(new Blob { Id = blobId });
        }
    }

    private async Task RemoveImageFile(Guid? sessionId, Guid? id)
    {
        var fetchResponse = await imageFileService.Fetch(new(sessionId, new() { Id = id }));
        if (!fetchResponse.Ok || fetchResponse.Value is null)
            return;

        var image = fetchResponse.Value;
        var removeResponse = await imageFileService.Remove(new(sessionId, image));
        if (!removeResponse.Ok)
            return;

        var derivedBlobIds = new[]
            {
                image.OptimizedBlobId,
                image.Resized96BlobId,
                image.Resized192BlobId,
                image.Resized360BlobId,
                image.Resized540BlobId,
                image.Resized720BlobId,
                image.Resized1080BlobId,
                image.Resized1440BlobId,
                image.Resized1920BlobId,
                image.Resized3840BlobId,
            }
            .Where(x => x.HasValue && x != image.BlobId)
            .Distinct();

        foreach (var blobId in derivedBlobIds)
            await blobService.Remove(new Blob { Id = blobId });
    }

    private static void ApplyStage(CommentMedia media, ForumUploadStage staged)
    {
        switch (media.Type)
        {
            case CommentMedia.Types.Audio:
                media.AudioFile.Id = null;
                media.AudioFile.BlobId = staged.BlobId;
                media.AudioFile.Name = staged.Name;
                media.AudioFile.Format = staged.Format;
                media.AudioFile.OptimizedStatus = AudioFile.OptimizationStatus.None;
                media.AudioFile.OptimizedBlobId = null;
                media.AudioFile.OptimizedFormat = null;
                break;
            case CommentMedia.Types.Image:
                media.ImageFile.Id = null;
                media.ImageFile.BlobId = staged.BlobId;
                media.ImageFile.Name = staged.Name;
                media.ImageFile.Format = staged.Format;
                media.ImageFile.OptimizedStatus = ImageFile.OptimizationStatus.None;
                media.ImageFile.OptimizedBlobId = null;
                media.ImageFile.OptimizedFormat = null;
                media.ImageFile.Resized96BlobId = null;
                media.ImageFile.Resized192BlobId = null;
                media.ImageFile.Resized360BlobId = null;
                media.ImageFile.Resized540BlobId = null;
                media.ImageFile.Resized720BlobId = null;
                media.ImageFile.Resized1080BlobId = null;
                media.ImageFile.Resized1440BlobId = null;
                media.ImageFile.Resized1920BlobId = null;
                media.ImageFile.Resized3840BlobId = null;
                media.ImageFile.Width = null;
                media.ImageFile.Height = null;
                break;
            case CommentMedia.Types.Pdf:
                media.PdfFile.Id = null;
                media.PdfFile.BlobId = staged.BlobId;
                media.PdfFile.Name = staged.Name;
                media.PdfFile.Format = staged.Format;
                break;
            case CommentMedia.Types.Video:
                media.VideoFile.Id = null;
                media.VideoFile.BlobId = staged.BlobId;
                media.VideoFile.Name = staged.Name;
                media.VideoFile.Format = staged.Format;
                media.VideoFile.OptimizedStatus = VideoFile.OptimizationStatus.None;
                media.VideoFile.OptimizedBlobId = null;
                media.VideoFile.OptimizedFormat = null;
                media.VideoFile.PosterBlobId = null;
                media.VideoFile.PosterFormat = null;
                media.VideoFile.Width = null;
                media.VideoFile.Height = null;
                break;
        }
    }

    private static async Task MergeMedia(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId,
        Comment comment, IEnumerable<CommentMedia> existingMedias, IEnumerable<Guid?> licenseIds)
    {
        var existing = existingMedias.ToList();
        var current = comment.CommentMedias.ToList();

        foreach (var removed in existing.Where(old => current.All(item => item.Id != old.Id)))
            await ForumRunCommentMediaWrite.Delete(connection, transaction, sessionId, removed.Id, licenseIds);

        for (var ordinal = 0; ordinal < current.Count; ordinal++)
        {
            var media = current[ordinal];
            media.CommentId = comment.Id;
            media.Ordinal = ordinal;

            if (existing.Any(x => x.Id == media.Id))
                await ForumRunCommentMediaWrite.Update(connection, transaction, sessionId, media, licenseIds);
            else
                media.Id = await ForumRunCommentMediaWrite.Insert(connection, transaction, sessionId, media, licenseIds);
        }
    }
}