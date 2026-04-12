create proc [FrameworkCore].[ImageFileInsert] (
     @SessionId uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
    ,@Width int
    ,@Height int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[ImageFile] (
    Id
    ,Updated
    ,UpdatedBy
    ,BlobId
    ,Name
    ,Format
    ,Width
    ,Height
    ,OptimizedStatus
)
values (
    @Id
    ,@now
    ,@SessionId
    ,@BlobId
    ,@Name
    ,@Format
    ,@Width
    ,@Height
    ,0
)