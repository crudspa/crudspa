create proc [EducationRostering].[RosterJobInsert] (
     @SessionId uniqueidentifier
    ,@DeviceId uniqueidentifier
    ,@RosterSourceId uniqueidentifier
    ,@JobId uniqueidentifier output
) as

set xact_abort on

declare @pending uniqueidentifier = '5e2d54a0-5774-4cae-8391-0b6ac31d4f60'
    ,@running uniqueidentifier = '28886325-475c-4d3e-9624-96e9c151775d'
    ,@scheduleId uniqueidentifier
    ,@typeId uniqueidentifier
    ,@provider nvarchar(75)

select @scheduleId = case when source.Recurring = 1 then source.ScheduleId end
    ,@provider = source.Provider
from [Education].[RosterSource-Active] source
where source.Id = @RosterSourceId and source.Mode <> N'disabled'

select @typeId = Id
from [Framework].[JobType-Active]
where Name = N'Roster Sync'

if @provider is null
    or @typeId is null
    or not exists (select 1 from [Framework].[Device-Active] where Id = @DeviceId)
    or exists (
        select 1
        from [Framework].[Job] with (updlock, holdlock)
        where IsDeleted = 0
            and TypeId = @typeId
            and StatusId in (@pending, @running)
            and try_convert(uniqueidentifier, json_value(Config, '$.SourceId')) = @RosterSourceId
)
begin
    set @JobId = null
    return
end

set @JobId = newid()

insert [Framework].[Job] (Id, TypeId, Config, Description, StatusId, DeviceId, ScheduleId)
values (
     @JobId
    ,@typeId
    ,concat(N'{"SourceId":"', convert(nvarchar(36), @RosterSourceId), N'","Kind":"full"}')
    ,concat(@provider, N' roster sync')
    ,@pending
    ,@DeviceId
    ,@scheduleId
)