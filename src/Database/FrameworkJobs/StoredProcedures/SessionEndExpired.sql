create proc [FrameworkJobs].[SessionEndExpired] (
     @SessionLengthInDays int
) as

set nocount on;

declare @now datetimeoffset(7) = sysdatetimeoffset();

update [Framework].[Session]
set IsDeleted = 1
    ,Ended = @now
where IsDeleted = 0
    and Ended is null
    and Started < dateadd(day, -@SessionLengthInDays, @now)