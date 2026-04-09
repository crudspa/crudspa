using AudioFileSelect = Crudspa.Framework.Jobs.Server.Sproxies.AudioFileSelect;
using ImageFileSelect = Crudspa.Framework.Jobs.Server.Sproxies.ImageFileSelect;
using ImageFileUpdate = Crudspa.Framework.Jobs.Server.Sproxies.ImageFileUpdate;
using VideoFileSelect = Crudspa.Framework.Jobs.Server.Sproxies.VideoFileSelect;

namespace Crudspa.Framework.Jobs.Server.Services;

public class FrameworkActionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IBlobService blobService,
    IHtmlSanitizer htmlSanitizer)
    : IFrameworkActionService
{
    private static IReadOnlyList<SanitizeTarget> SanitizeTargets { get; } =
    [
        new("Achievement.Description", true, """
            select {0} root.Id, root.[Description] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Achievement] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Description] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Achievement]
            set [Description] = @Html
            where [Id] = @Id
            """),
        new("Blog.Description", true, """
            select {0} root.Id, root.[Description] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Blog] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Description] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Blog]
            set [Description] = @Html
            where [Id] = @Id
            """),
        new("Comment.Body", false, """
            select {0} root.Id, root.[Body] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Comment] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Body] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Comment]
            set [Body] = @Html
            where [Id] = @Id
            """),
        new("Course.Description", false, """
            select {0} root.Id, root.[Description] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Course] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Description] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Course]
            set [Description] = @Html
            where [Id] = @Id
            """),
        new("Email.Body", false, """
            select {0} root.Id, root.[Body] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Email] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Body] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Email]
            set [Body] = @Html
            where [Id] = @Id
            """),
        new("EmailTemplate.Body", false, """
            select {0} root.Id, root.[Body] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[EmailTemplate] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Body] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[EmailTemplate]
            set [Body] = @Html
            where [Id] = @Id
            """),
        new("Forum.Description", false, """
            select {0} root.Id, root.[Description] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Forum] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Description] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Forum]
            set [Description] = @Html
            where [Id] = @Id
            """),
        new("Membership.Description", true, """
            select {0} root.Id, root.[Description] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Membership] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Description] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Membership]
            set [Description] = @Html
            where [Id] = @Id
            """),
        new("MultimediaItem.Text", true, """
            select {0} root.Id, root.[Text] as Html, section.PageId as PageId
            from [Content].[MultimediaItem] root
                inner join [Content].[MultimediaElement-Active] multimediaElement on multimediaElement.Id = root.MultimediaElementId
                inner join [Content].[Element-Active] element on element.Id = multimediaElement.ElementId
                inner join [Content].[Section-Active] section on section.Id = element.SectionId
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Text] is not null
                and root.[MediaTypeIndex] = 3
            order by root.[Updated] desc
            """, """
            update [Content].[MultimediaItem]
            set [Text] = @Html
            where [Id] = @Id
            """),
        new("TextElement.Text", false, """
            select {0} root.Id, root.[Text] as Html, section.PageId as PageId
            from [Content].[TextElement] root
                inner join [Content].[Element-Active] element on element.Id = root.ElementId
                inner join [Content].[Section-Active] section on section.Id = element.SectionId
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Text] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[TextElement]
            set [Text] = @Html
            where [Id] = @Id
            """),
        new("Track.Description", false, """
            select {0} root.Id, root.[Description] as Html, cast(null as uniqueidentifier) as PageId
            from [Content].[Track] root
            where root.VersionOf = root.Id
                and root.IsDeleted = 0
                and root.[Description] is not null
            order by root.[Updated] desc
            """, """
            update [Content].[Track]
            set [Description] = @Html
            where [Id] = @Id
            """),
    ];

    private String Connection => configService.Fetch().Database;

    public async Task<Response> ExpireSessions(Request request, Int32? sessionLengthInDays)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await SessionEndExpired.Execute(connection, transaction, sessionLengthInDays));
        });
    }

    public async Task<Response<SanitizeHtmlRun>> SanitizeHtml(Request request, SanitizeHtmlConfig config)
    {
        return await wrappers.Try<SanitizeHtmlRun>(request, async response =>
        {
            config ??= new();

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var run = new SanitizeHtmlRun();

                foreach (var target in SanitizeTargets)
                {
                    var targetRun = await Sanitize(connection, transaction, target, config);
                    run.Targets.Add(targetRun);
                    run.RowsScanned += targetRun.RowsScanned;
                    run.RowsUpdated += targetRun.RowsUpdated;
                    run.RowsSkippedRequired += targetRun.RowsSkippedRequired;
                }

                run.AffectedPageIds = run.Targets
                    .SelectMany(x => x.AffectedPageIds)
                    .Distinct()
                    .ToObservable();

                return run;
            });
        });
    }

    public async Task<Response<IList<AudioFile>>> FetchAudioForOptimization(Request request)
    {
        return await wrappers.Try<IList<AudioFile>>(request, async response =>
            await AudioFileSelectForOptimization.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<AudioFile>>> FetchAudioBeenOptimized(Request request)
    {
        return await wrappers.Try<IList<AudioFile>>(request, async response =>
            await AudioFileSelectBeenOptimized.Execute(Connection));
    }

    public async Task<Response> SaveAudioOptimizationStatus(Request<AudioFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var audioFile = request.Value;

            var existingAudioFile = await AudioFileSelect.Execute(Connection, audioFile);

            if (audioFile.OptimizedStatus != AudioFile.OptimizationStatus.Succeeded
                && existingAudioFile?.OptimizedBlobId is not null)
            {
                await blobService.Remove(new Blob { Id = existingAudioFile.OptimizedBlobId });
                audioFile.OptimizedBlobId = null;
                audioFile.OptimizedFormat = null;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await AudioFileUpdateOptimizationStatus.Execute(connection, transaction, request.SessionId, request.Value));
        });
    }

    public async Task<Response<IList<ImageFile>>> FetchImageForOptimization(Request request)
    {
        return await wrappers.Try<IList<ImageFile>>(request, async response =>
            await ImageFileSelectForOptimization.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<ImageFile>>> FetchImageBeenOptimized(Request request)
    {
        return await wrappers.Try<IList<ImageFile>>(request, async response =>
            await ImageFileSelectBeenOptimized.Execute(Connection));
    }

    public async Task<Response> SaveImageOptimizationStatus(Request<ImageFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var imageFile = request.Value;

            var existingImageFile = await ImageFileSelect.Execute(Connection, imageFile);

            if (existingImageFile is not null && imageFile.OptimizedStatus != ImageFile.OptimizationStatus.Succeeded)
            {
                if (existingImageFile.OptimizedBlobId is not null)
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.OptimizedBlobId });
                    imageFile.OptimizedBlobId = null;
                }

                if (existingImageFile.Resized96BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized96BlobId });
                    imageFile.Resized96BlobId = null;
                }

                if (existingImageFile.Resized192BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized192BlobId });
                    imageFile.Resized192BlobId = null;
                }

                if (existingImageFile.Resized360BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized360BlobId });
                    imageFile.Resized360BlobId = null;
                }

                if (existingImageFile.Resized540BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized540BlobId });
                    imageFile.Resized540BlobId = null;
                }

                if (existingImageFile.Resized720BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized720BlobId });
                    imageFile.Resized720BlobId = null;
                }

                if (existingImageFile.Resized1080BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized1080BlobId });
                    imageFile.Resized1080BlobId = null;
                }

                if (existingImageFile.Resized1440BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized1440BlobId });
                    imageFile.Resized1440BlobId = null;
                }

                if (existingImageFile.Resized1920BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized1920BlobId });
                    imageFile.Resized1920BlobId = null;
                }

                if (existingImageFile.Resized3840BlobId.HasSomething())
                {
                    await blobService.Remove(new Blob { Id = existingImageFile.Resized3840BlobId });
                    imageFile.Resized3840BlobId = null;
                }
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await ImageFileUpdateOptimizationStatus.Execute(connection, transaction, request.SessionId, request.Value));
        });
    }

    public async Task<Response<IList<ImageFile>>> FetchImageForCaptioning(Request request)
    {
        return await wrappers.Try<IList<ImageFile>>(request, async response =>
            await ImageFileSelectForCaptioning.Execute(Connection, request.SessionId));
    }

    public async Task<Response> SaveImageCaption(Request<ImageFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var imageFile = request.Value;

            var existingImageFile = await ImageFileSelect.Execute(Connection, imageFile);

            if (existingImageFile is not null)
                if (imageFile.BlobId != existingImageFile.BlobId)
                    await blobService.Remove(new Blob { Id = existingImageFile.BlobId });

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await ImageFileUpdate.Execute(connection, transaction, request.SessionId, imageFile));
        });
    }

    public async Task<Response<IList<VideoFile>>> FetchVideoForOptimization(Request request)
    {
        return await wrappers.Try<IList<VideoFile>>(request, async response =>
            await VideoFileSelectForOptimization.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<VideoFile>>> FetchVideoBeenOptimized(Request request)
    {
        return await wrappers.Try<IList<VideoFile>>(request, async response =>
            await VideoFileSelectBeenOptimized.Execute(Connection));
    }

    public async Task<Response> SaveVideoOptimizationStatus(Request<VideoFile> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var videoFile = request.Value;

            var existingVideoFile = await VideoFileSelect.Execute(Connection, videoFile);

            if (existingVideoFile is not null)
            {
                if (videoFile.OptimizedStatus != VideoFile.OptimizationStatus.Succeeded)
                {
                    if (existingVideoFile.OptimizedBlobId is not null)
                        await blobService.Remove(new Blob { Id = existingVideoFile.OptimizedBlobId });

                    if (existingVideoFile.PosterBlobId is not null)
                        await blobService.Remove(new Blob { Id = existingVideoFile.PosterBlobId });

                    videoFile.OptimizedBlobId = null;
                    videoFile.OptimizedFormat = null;
                    videoFile.PosterBlobId = null;
                    videoFile.PosterFormat = null;
                }
                else
                {
                    if (existingVideoFile.OptimizedBlobId is not null
                        && existingVideoFile.OptimizedBlobId != videoFile.OptimizedBlobId)
                        await blobService.Remove(new Blob { Id = existingVideoFile.OptimizedBlobId });

                    if (existingVideoFile.PosterBlobId is not null
                        && existingVideoFile.PosterBlobId != videoFile.PosterBlobId)
                        await blobService.Remove(new Blob { Id = existingVideoFile.PosterBlobId });
                }
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
                await VideoFileUpdateOptimizationStatus.Execute(connection, transaction, request.SessionId, request.Value));
        });
    }

    private async Task<SanitizeHtmlTargetRun> Sanitize(SqlConnection connection, SqlTransaction? transaction, SanitizeTarget target, SanitizeHtmlConfig config)
    {
        var topClause = config.LimitRowsPerTarget is int limit ? $"top ({limit})" : String.Empty;
        var targetRun = new SanitizeHtmlTargetRun { Name = target.Name };
        var rows = new List<SanitizeRow>();

        await using (var command = new SqlCommand(String.Format(target.SelectSql, topClause), connection, transaction))
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                rows.Add(new(
                    reader.GetGuid(0),
                    reader.IsDBNull(1) ? null : reader.GetString(1),
                    reader.IsDBNull(2) ? null : reader.GetGuid(2)));
            }
        }

        targetRun.RowsScanned = rows.Count;

        foreach (var row in rows)
        {
            var sanitized = htmlSanitizer.Sanitize(row.Html);

            if (sanitized is null && !target.AllowNull)
            {
                targetRun.RowsSkippedRequired++;
                continue;
            }

            if (sanitized.IsExactly(row.Html))
                continue;

            await using var update = new SqlCommand(target.UpdateSql, connection, transaction);
            update.Parameters.AddWithValue("@Id", row.Id);
            update.Parameters.AddWithValue("@Html", sanitized is null ? DBNull.Value : sanitized);
            await update.ExecuteNonQueryAsync();

            targetRun.RowsUpdated++;

            if (row.PageId.HasValue)
                targetRun.AffectedPageIds.Add(row.PageId.Value);
        }

        return targetRun;
    }

    private readonly record struct SanitizeTarget(String Name, Boolean AllowNull, String SelectSql, String UpdateSql);
    private readonly record struct SanitizeRow(Guid Id, String? Html, Guid? PageId);
}