create proc [FrameworkCore].[FontFileInsert] (
     @SessionId uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
    ,@Description nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Framework].[FontFile] (
    Id
    ,Updated
    ,UpdatedBy
    ,BlobId
    ,Name
    ,Format
    ,Description
)
values (
    @Id
    ,@now
    ,@SessionId
    ,@BlobId
    ,@Name
    ,@Format
    ,@Description
)