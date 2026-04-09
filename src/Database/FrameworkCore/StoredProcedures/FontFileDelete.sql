create proc [FrameworkCore].[FontFileDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[FontFile]
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id