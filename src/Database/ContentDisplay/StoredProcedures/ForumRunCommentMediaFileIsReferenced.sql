create proc [ContentDisplay].[ForumRunCommentMediaFileIsReferenced] (
     @Type int
    ,@FileId uniqueidentifier
) as
begin
    set nocount on;

    if exists (
        select 1
        from [Content].[CommentMedia-Active]
        where ([Type] = 0 and @Type = 0 and AudioId = @FileId)
            or ([Type] = 1 and @Type = 1 and ImageId = @FileId)
            or ([Type] = 2 and @Type = 2 and PdfId = @FileId)
            or ([Type] = 3 and @Type = 3 and VideoId = @FileId)
    ) or (
        @Type = 1
        and exists (
            select 1
            from [Content].[Forum-Active]
            where ImageId = @FileId
        )
    )
    begin
        select convert(bit, 1);
        return;
    end

    create table #forumMediaBlobIds (Id uniqueidentifier not null primary key);

    if @Type = 0
        insert #forumMediaBlobIds (Id)
        select distinct blob.Id
        from [Framework].[AudioFile] audio
        cross apply (values (audio.BlobId), (audio.OptimizedBlobId)) blob(Id)
        where audio.Id = @FileId and blob.Id is not null;
    else if @Type = 1
        insert #forumMediaBlobIds (Id)
        select distinct blob.Id
        from [Framework].[ImageFile] image
        cross apply (values
             (image.BlobId)
            ,(image.OptimizedBlobId)
            ,(image.Resized96BlobId)
            ,(image.Resized192BlobId)
            ,(image.Resized360BlobId)
            ,(image.Resized540BlobId)
            ,(image.Resized720BlobId)
            ,(image.Resized1080BlobId)
            ,(image.Resized1440BlobId)
            ,(image.Resized1920BlobId)
            ,(image.Resized3840BlobId)
        ) blob(Id)
        where image.Id = @FileId and blob.Id is not null;
    else if @Type = 2
        insert #forumMediaBlobIds (Id)
        select pdf.BlobId
        from [Framework].[PdfFile] pdf
        where pdf.Id = @FileId;
    else if @Type = 3
        insert #forumMediaBlobIds (Id)
        select distinct blob.Id
        from [Framework].[VideoFile] video
        cross apply (values (video.BlobId), (video.OptimizedBlobId), (video.PosterBlobId)) blob(Id)
        where video.Id = @FileId and blob.Id is not null;

    select convert(bit, case when exists (
        select 1
        from [Framework].[AudioFile-Active] audio
        cross apply (values (audio.BlobId), (audio.OptimizedBlobId)) blob(Id)
        inner join #forumMediaBlobIds targetBlob on targetBlob.Id = blob.Id
        where @Type <> 0 or audio.Id <> @FileId
        union all
        select 1
        from [Framework].[ImageFile-Active] image
        cross apply (values
             (image.BlobId)
            ,(image.OptimizedBlobId)
            ,(image.Resized96BlobId)
            ,(image.Resized192BlobId)
            ,(image.Resized360BlobId)
            ,(image.Resized540BlobId)
            ,(image.Resized720BlobId)
            ,(image.Resized1080BlobId)
            ,(image.Resized1440BlobId)
            ,(image.Resized1920BlobId)
            ,(image.Resized3840BlobId)
        ) blob(Id)
        inner join #forumMediaBlobIds targetBlob on targetBlob.Id = blob.Id
        where @Type <> 1 or image.Id <> @FileId
        union all
        select 1
        from [Framework].[PdfFile-Active] pdf
        inner join #forumMediaBlobIds targetBlob on targetBlob.Id = pdf.BlobId
        where @Type <> 2 or pdf.Id <> @FileId
        union all
        select 1
        from [Framework].[VideoFile-Active] video
        cross apply (values (video.BlobId), (video.OptimizedBlobId), (video.PosterBlobId)) blob(Id)
        inner join #forumMediaBlobIds targetBlob on targetBlob.Id = blob.Id
        where @Type <> 3 or video.Id <> @FileId
        union all
        select 1
        from [Framework].[FontFile-Active] font
        inner join #forumMediaBlobIds targetBlob on targetBlob.Id = font.BlobId
        union all
        select 1
        from [Framework].[TextFile-Active] textFile
        inner join #forumMediaBlobIds targetBlob on targetBlob.Id = textFile.BlobId
        union all
        select 1
        from [Framework].[ExportFile-Active] exportFile
        inner join #forumMediaBlobIds targetBlob on targetBlob.Id = exportFile.BlobId
    ) then 1 else 0 end);
end