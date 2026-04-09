create proc [FrameworkCore].[SessionEnd] (
     @Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[Session]
set  IsDeleted = 1
    ,Ended = @now
where Id = @Id
    and IsDeleted = 0