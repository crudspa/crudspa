create proc [FrameworkJobs].[JobInsert] (
     @SessionId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Config nvarchar(max)
    ,@Description nvarchar(max)
    ,@DeviceId uniqueidentifier
    ,@ScheduleId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @JobStatusPending uniqueidentifier = '5e2d54a0-5774-4cae-8391-0b6ac31d4f60'

insert [Framework].[Job] (
     Id
    ,TypeId
    ,Config
    ,Description
    ,StatusId
    ,DeviceId
    ,ScheduleId
)
values (
     @Id
    ,@TypeId
    ,@Config
    ,@Description
    ,@JobStatusPending
    ,@DeviceId
    ,@ScheduleId
)