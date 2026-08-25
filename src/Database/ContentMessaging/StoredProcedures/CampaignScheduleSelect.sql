create proc [ContentMessaging].[CampaignScheduleSelect] (
     @SessionId uniqueidentifier
    ,@ActivationId uniqueidentifier
) as

set nocount on

declare @districtPortalId uniqueidentifier = '18da2a92-c650-42fb-8ff9-07c81ab5b9b2'
declare @organizationId uniqueidentifier = (
    select OrganizationId from [Content].[Activation-Active] where Id = @ActivationId
)

if @organizationId is null or not exists (
    select 1 from [ContentMessaging].[SessionCanReadOrganization](@SessionId, @districtPortalId, @organizationId)
)
    throw 51000, 'Campaign schedule access denied.', 1

select
     activation.Id
    ,activation.CampaignId
    ,campaign.Name
    ,activation.OrganizationId
    ,organization.Name
    ,status.Name
from [Content].[Activation-Active] activation
    inner join [Content].[Campaign-Active] campaign on campaign.Id = activation.CampaignId
    inner join [Framework].[Organization-Active] organization on organization.Id = activation.OrganizationId
    inner join [Content].[ActivationStatus-Active] status on status.Id = activation.StatusId
where activation.Id = @ActivationId

select
     stage.Id
    ,stage.Name
    ,stage.Offset
    ,stage.Anchor
    ,stage.SendTime
    ,stage.WeekendAdjustment
    ,stagePlan.ScopeLevel
from [Content].[Activation-Active] activation
    inner join [Content].[Stage-Active] stage on stage.CampaignId = activation.CampaignId
    inner join [Content].[ActivationStagePlan-Active] stagePlan on stagePlan.ActivationId = activation.Id and stagePlan.StageId = stage.Id
where activation.Id = @ActivationId
order by stage.Ordinal

select
     scope.Id
    ,scope.ParentId
    ,scope.OrganizationId
    ,organization.Name
    ,schedule.GradeId
    ,grade.Name
    ,scope.Name
    ,scope.Start
    ,scope.StartOverridden
    ,schedule.LessonStart
    ,schedule.LessonStartOverridden
    ,schedule.AssessmentStart
    ,schedule.AssessmentStartOverridden
    ,scope.Ordinal
from [Content].[ActivationScope-Active] scope
    inner join [Framework].[Organization-Active] organization on organization.Id = scope.OrganizationId
    inner join [Content].[CampaignSchedule-Active] schedule on schedule.ActivationScopeId = scope.Id
    left join [Education].[Grade-Active] grade on grade.Id = schedule.GradeId
where scope.ActivationId = @ActivationId
order by scope.Ordinal

select
     activationStage.ActivationScopeId
    ,stagePlan.StageId
    ,activationStage.Send
    ,activationStage.Overridden
    ,organization.TimeZoneId
from [Content].[ActivationStagePlan-Active] stagePlan
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.PlanId = stagePlan.Id
    inner join [Content].[ActivationScope-Active] scope on scope.Id = activationStage.ActivationScopeId
    inner join [Framework].[Organization-Active] organization on organization.Id = scope.OrganizationId
where stagePlan.ActivationId = @ActivationId

exec [ContentMessaging].[CampaignScheduleOptionSelect] @SessionId, @organizationId