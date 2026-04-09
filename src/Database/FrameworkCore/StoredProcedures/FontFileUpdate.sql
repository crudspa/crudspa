create proc [FrameworkCore].[FontFileUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
    ,@Description nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[FontFile]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,BlobId = @BlobId
    ,Name = @Name
    ,Format = @Format
    ,Description = @Description
where Id = @Id