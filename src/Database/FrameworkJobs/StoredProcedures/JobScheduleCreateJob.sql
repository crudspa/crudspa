create proc [FrameworkJobs].[JobScheduleCreateJob] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@NextRun datetimeoffset(7)
    ,@JobId uniqueidentifier output
) as

declare @now datetimeoffset = sysdatetimeoffset()
set @JobId = newid()

update [Framework].[JobSchedule]
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,NextRun = @NextRun
where Id = @Id

declare @JobStatusPending uniqueidentifier = '5e2d54a0-5774-4cae-8391-0b6ac31d4f60'
declare @JobStatusRunning uniqueidentifier = '28886325-475c-4d3e-9624-96e9c151775d'

insert [Framework].[Job] (
     Id
    ,TypeId
    ,Config
    ,Description
    ,StatusId
    ,DeviceId
    ,ScheduleId
)
select
     @JobId
    ,jobSchedule.TypeId
    ,jobSchedule.Config
    ,jobSchedule.Description
    ,@JobStatusPending
    ,jobSchedule.DeviceId
    ,@Id
from [Framework].[JobSchedule] jobSchedule
where jobSchedule.Id = @Id
    and not exists (
        select 1
        from [Framework].[Job-Active] job
        where job.ScheduleId = @Id
            and job.StatusId in (@JobStatusPending, @JobStatusRunning)
    )

if @@rowcount = 0
    set @JobId = null