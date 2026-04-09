create proc [FrameworkJobs].[ImageFileUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BlobId uniqueidentifier
    ,@Name nvarchar(150)
    ,@Format nvarchar(10)
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
    ,Caption = @Caption
where Id = @Id