create proc [FrameworkJobs].[JobSelectSingle] (
     @SessionId uniqueidentifier
    ,@DeviceId uniqueidentifier
    ,@JobId uniqueidentifier
) as

declare @JobStatusPending uniqueidentifier = '5e2d54a0-5774-4cae-8391-0b6ac31d4f60'
declare @JobStatusRunning uniqueidentifier = '28886325-475c-4d3e-9624-96e9c151775d'

declare @now datetimeoffset = sysdatetimeoffset()
declare @batchId uniqueidentifier = newid()

update job
set  job.Started = @now
    ,job.StatusId = @JobStatusRunning
    ,job.DeviceId = @DeviceId
    ,job.BatchId = @batchId
from [Framework].[Job] job
    inner join [Framework].[DeviceJobType] deviceJobType on deviceJobType.JobTypeId = job.TypeId
where job.IsDeleted = 0
    and job.StatusId = @JobStatusPending
    and job.BatchId is null
    and (job.DeviceId is null or job.DeviceId = @DeviceId)
    and deviceJobType.DeviceId = @DeviceId
    and job.Id = @JobId

select top 1
     job.Id
    ,job.TypeId
    ,job.Config
    ,job.Added
    ,job.Started
    ,job.Ended
    ,job.StatusId
    ,job.DeviceId
    ,job.ScheduleId
    ,job.BatchId
    ,jobType.Id as JobTypeId
    ,jobType.Name as JobTypeName
    ,jobType.EditorView as JobTypeEditorView
    ,jobType.ActionClass as JobTypeActionClass
from [Framework].[Job-Active] job
    inner join [Framework].[JobType-Active] jobType on job.TypeId = jobType.Id
where job.BatchId = @batchId
order by job.Added