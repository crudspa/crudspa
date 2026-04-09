create proc [FrameworkJobs].[JobScheduleUpdateNextRun] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@NextRun datetimeoffset(7)
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[JobSchedule]
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,NextRun = @NextRun
where Id = @Id