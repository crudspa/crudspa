create proc [FrameworkJobs].[JobCancelRunning] (
     @DeviceId uniqueidentifier
) as

declare @JobStatusRunning uniqueidentifier = '28886325-475c-4d3e-9624-96e9c151775d'
declare @JobStatusCanceled uniqueidentifier = '3461ccbd-4ceb-4de4-94e5-0c6b3e36ae9d'

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[Job]
set  StatusId = @JobStatusCanceled
    ,Ended = @now
where StatusId = @JobStatusRunning
    and DeviceId = @DeviceId