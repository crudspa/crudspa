create proc [FrameworkJobs].[JobScheduleDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[JobSchedule]
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id