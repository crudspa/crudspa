create proc [FrameworkCore].[AudioFileDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[AudioFile]
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id