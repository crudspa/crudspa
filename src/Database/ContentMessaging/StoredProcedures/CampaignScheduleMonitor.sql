create proc [ContentMessaging].[CampaignScheduleMonitor] as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @today date = convert(date, @now)
declare @scheduled uniqueidentifier = 'a9b01002-7c15-4fb8-b25f-9f5629031002'
declare @active uniqueidentifier = 'a9b01003-7c15-4fb8-b25f-9f5629031003'
declare @completed uniqueidentifier = 'a9b01004-7c15-4fb8-b25f-9f5629031004'
declare @needsAttention uniqueidentifier = 'a9b01005-7c15-4fb8-b25f-9f5629031005'

declare @status table (ActivationId uniqueidentifier primary key, StatusId uniqueidentifier not null)

insert @status (ActivationId, StatusId)
select
     activation.Id
    ,case
        when (select count(*) from [Content].[ActivationScope-Active] scope where scope.ActivationId = activation.Id and scope.ParentId is null) <> 1
          or exists (
              select 1 from [Content].[ActivationScope-Active] scope
              where scope.ActivationId = activation.Id
                and not exists (select 1 from [Content].[CampaignSchedule-Active] schedule where schedule.ActivationScopeId = scope.Id)
          )
          or exists (
              select 1
              from [Content].[ActivationScope-Active] scope
                  cross join [Content].[Stage-Active] stage
              where scope.ActivationId = activation.Id and stage.CampaignId = activation.CampaignId
                and not exists (
                    select 1
                    from [Content].[ActivationStagePlan-Active] stagePlan
                        inner join [Content].[ActivationStage-Active] occurrence on occurrence.PlanId = stagePlan.Id
                    where stagePlan.ActivationId = activation.Id and stagePlan.StageId = stage.Id
                        and occurrence.ActivationScopeId = scope.Id
                )
          ) then @needsAttention
        when not exists (
            select 1 from [Content].[Message-Active] message
            where message.ActivationId = activation.Id
                and (
                    (message.EmailId is not null and exists (select 1 from [Content].[Email-Active] email where email.Id = message.EmailId and email.Status = 0))
                    or (message.SmsId is not null and exists (select 1 from [Content].[Sms-Active] sms where sms.Id = message.SmsId and sms.Status = 0))
                )
        ) then @completed
        when activation.Start <= @today
          or exists (
              select 1
              from [Content].[ActivationStagePlan-Active] stagePlan
                  inner join [Content].[ActivationStage-Active] occurrence on occurrence.PlanId = stagePlan.Id
              where stagePlan.ActivationId = activation.Id and occurrence.Send <= @now
          ) then @active
        else @scheduled
     end
from [Content].[Activation-Active] activation

update activation set
     StatusId = wanted.StatusId
    ,Updated = @now
from [Content].[Activation] activation
    inner join @status wanted on wanted.ActivationId = activation.Id
where activation.VersionOf = activation.Id and activation.StatusId <> wanted.StatusId

select
     count(*) as Monitored
    ,coalesce(sum(case when StatusId = @scheduled then 1 else 0 end), 0) as Scheduled
    ,coalesce(sum(case when StatusId = @active then 1 else 0 end), 0) as Active
    ,coalesce(sum(case when StatusId = @completed then 1 else 0 end), 0) as Completed
    ,coalesce(sum(case when StatusId = @needsAttention then 1 else 0 end), 0) as NeedsAttention
from @status