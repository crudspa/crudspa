create proc [FrameworkCore].[AudioFileInsert] (
     @SessionId uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
    ,@OptimizedStatus int
    ,@OptimizedBlobId uniqueidentifier
    ,@OptimizedFormat nvarchar(10)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[AudioFile] (
    Id
    ,Updated
    ,UpdatedBy
    ,BlobId
    ,Name
    ,Format
    ,OptimizedStatus
    ,OptimizedBlobId
    ,OptimizedFormat
)
values (
    @Id
    ,@now
    ,@SessionId
    ,@BlobId
    ,@Name
    ,@Format
    ,@OptimizedStatus
    ,@OptimizedBlobId
    ,@OptimizedFormat
)