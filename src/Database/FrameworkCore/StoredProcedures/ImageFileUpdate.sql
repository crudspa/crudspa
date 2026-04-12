create proc [FrameworkCore].[ImageFileUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
    ,@Width int
    ,@Height int
    ,@Caption nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[ImageFile]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,BlobId = @BlobId
    ,Name = @Name
    ,Format = @Format
    ,Width = @Width
    ,Height = @Height
    ,Caption = @Caption
where Id = @Id