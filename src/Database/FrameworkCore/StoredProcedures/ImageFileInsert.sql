create proc [FrameworkCore].[ImageFileInsert] (
     @SessionId uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
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
    ,OptimizedStatus
)
values (
    @Id
    ,@now
    ,@SessionId
    ,@BlobId
    ,@Name
    ,@Format
    ,0
)