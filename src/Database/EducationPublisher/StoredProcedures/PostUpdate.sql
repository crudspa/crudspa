create proc [EducationPublisher].[PostUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Pinned bit
    ,@Body nvarchar(max)
    ,@AudioId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@VideoId uniqueidentifier
    ,@Type int
    ,@GradeId uniqueidentifier
    ,@SubjectId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[Post]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Pinned = @Pinned
    ,Body = @Body
    ,AudioId = @AudioId
    ,ImageId = @ImageId
    ,PdfId = @PdfId
    ,VideoId = @VideoId
    ,Type = @Type
    ,GradeId = @GradeId
    ,SubjectId = @SubjectId
    ,Edited = @now
where Id = @Id