create proc [ContentMessaging].[CampaignScheduleSave] (
     @SessionId uniqueidentifier
    ,@ActivationId uniqueidentifier
    ,@FromName nvarchar(150)
    ,@FromEmail nvarchar(75)
    ,@Scopes [ContentMessaging].[CampaignScopeList] readonly
    ,@Schedules [ContentMessaging].[ScopedStageScheduleList] readonly
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @organizationId uniqueidentifier
declare @campaignId uniqueidentifier
declare @portalId uniqueidentifier
declare @stageCount int

select
     @organizationId = activation.OrganizationId
    ,@campaignId = activation.CampaignId
    ,@portalId = campaign.PortalId
from [Content].[Activation-Active] activation
    inner join [Content].[Campaign-Active] campaign on campaign.Id = activation.CampaignId
where activation.Id = @ActivationId

if @organizationId is null or not exists (
    select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, @portalId, @organizationId)
)
    throw 51000, 'Campaign schedule access denied.', 1

if (select count(*) from @Scopes where ParentScopeKey is null) <> 1
    throw 51000, 'Exactly one district-wide schedule is required.', 1

if exists (select 1 from @Scopes where DistrictOrganizationId <> @organizationId)
    throw 51000, 'A schedule scope belongs to a different District.', 1

if exists (
    select 1
    from @Scopes scope
    where scope.OrganizationId <> @organizationId
        and not exists (
            select 1
            from [Education].[School-Active] school
                inner join [Education].[District-Active] district on district.Id = school.DistrictId
            where school.OrganizationId = scope.OrganizationId
                and district.OrganizationId = @organizationId
        )
)
    throw 51000, 'A school schedule belongs to a different District.', 1

if exists (
    select 1 from @Scopes scope
    where (scope.ParentScopeKey is null and (scope.ScopeKey not in (
                select Id from [Content].[ActivationScope-Active] where ActivationId = @ActivationId and ParentId is null
            ) or scope.GradeId is not null or scope.OrganizationId <> @organizationId))
       or (scope.ParentScopeKey is not null and scope.GradeId is null)
)
    throw 51000, 'The submitted schedule scope hierarchy is invalid.', 1

if exists (
    select 1 from @Scopes scope
    where scope.ParentScopeKey is not null
        and not exists (select 1 from @Scopes parent where parent.ScopeKey = scope.ParentScopeKey)
)
    throw 51000, 'A schedule references an unavailable parent scope.', 1

if exists (
    select 1 from @Scopes
    where ParentScopeKey is not null
    group by OrganizationId, GradeId
    having count(*) > 1
)
    throw 51000, 'A grade or school schedule is duplicated.', 1

if exists (
    select 1
    from @Scopes scope
        cross join (select ScopeKey from @Scopes where ParentScopeKey is null) root
        outer apply (
            select ScopeKey
            from @Scopes grade
            where grade.OrganizationId = grade.DistrictOrganizationId
                and grade.GradeId = scope.GradeId
                and grade.ParentScopeKey is not null
        ) grade
    where scope.ParentScopeKey is not null
        and (
            (scope.OrganizationId = scope.DistrictOrganizationId and scope.ParentScopeKey <> root.ScopeKey)
            or
            (scope.OrganizationId <> scope.DistrictOrganizationId
                and scope.ParentScopeKey <> coalesce(grade.ScopeKey, root.ScopeKey))
        )
)
    throw 51000, 'A grade or school schedule has an invalid parent schedule.', 1

select @stageCount = count(*) from [Content].[Stage-Active] where CampaignId = @campaignId
if @stageCount * (select count(*) from @Scopes) <> (select count(*) from @Schedules)
    throw 51000, 'Every scope requires a date for every Campaign Stage.', 1

if exists (
    select 1 from @Schedules schedule
    left join [Content].[Stage-Active] stage on stage.Id = schedule.StageId and stage.CampaignId = @campaignId
    where stage.Id is null
)
    throw 51000, 'A schedule references an unavailable Campaign Stage.', 1

declare @newScopes table (Id uniqueidentifier primary key)
insert @newScopes (Id)
select ScopeKey from @Scopes
where not exists (select 1 from [Content].[ActivationScope] existing where existing.Id = ScopeKey)

begin transaction

-- Retire removed schedules and their unsent realized work.
update email set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
from [Content].[Email] email
    inner join [Content].[Message-Active] message on message.EmailId = email.Id
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.Id = message.ActivationStageId
    inner join [Content].[ActivationScope-Active] scope on scope.Id = activationStage.ActivationScopeId
where scope.ActivationId = @ActivationId
    and not exists (select 1 from @Scopes wanted where wanted.ScopeKey = scope.Id)
    and email.Status = 0

update sms set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
from [Content].[Sms] sms
    inner join [Content].[Message-Active] message on message.SmsId = sms.Id
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.Id = message.ActivationStageId
    inner join [Content].[ActivationScope-Active] scope on scope.Id = activationStage.ActivationScopeId
where scope.ActivationId = @ActivationId
    and not exists (select 1 from @Scopes wanted where wanted.ScopeKey = scope.Id)
    and sms.Status = 0

update message set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
from [Content].[Message] message
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.Id = message.ActivationStageId
    inner join [Content].[ActivationScope-Active] scope on scope.Id = activationStage.ActivationScopeId
where scope.ActivationId = @ActivationId
    and not exists (select 1 from @Scopes wanted where wanted.ScopeKey = scope.Id)

update membership set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
from [Content].[Membership] membership
    inner join [Content].[ActivationScope-Active] scope on scope.Id = membership.ActivationScopeId
where scope.ActivationId = @ActivationId
    and not exists (select 1 from @Scopes wanted where wanted.ScopeKey = scope.Id)

update activationStage set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
from [Content].[ActivationStage] activationStage
    inner join [Content].[ActivationScope-Active] scope on scope.Id = activationStage.ActivationScopeId
where scope.ActivationId = @ActivationId
    and not exists (select 1 from @Scopes wanted where wanted.ScopeKey = scope.Id)

update campaignSchedule set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
from [Content].[CampaignSchedule] campaignSchedule
    inner join [Content].[ActivationScope-Active] scope on scope.Id = campaignSchedule.ActivationScopeId
where scope.ActivationId = @ActivationId
    and not exists (select 1 from @Scopes wanted where wanted.ScopeKey = scope.Id)

update scope set IsDeleted = 1, Updated = @now, UpdatedBy = @SessionId
from [Content].[ActivationScope] scope
where scope.ActivationId = @ActivationId and scope.ParentId is not null
    and not exists (select 1 from @Scopes wanted where wanted.ScopeKey = scope.Id)

-- Update retained scopes and add new schedules.
update target set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,IsDeleted = 0
    ,ParentId = wanted.ParentScopeKey
    ,OrganizationId = wanted.OrganizationId
    ,Name = wanted.Name
    ,Start = wanted.Start
    ,StartOverridden = wanted.StartOverridden
    ,Ordinal = wanted.Ordinal
from [Content].[ActivationScope] target
    inner join @Scopes wanted on wanted.ScopeKey = target.Id
where target.VersionOf = target.Id

insert [Content].[ActivationScope] (
     Id, VersionOf, Updated, UpdatedBy, ActivationId, ParentId, OrganizationId,
     Name, Start, StartOverridden, Ordinal
)
select
     wanted.ScopeKey, wanted.ScopeKey, @now, @SessionId, @ActivationId, wanted.ParentScopeKey,
     wanted.OrganizationId, wanted.Name, wanted.Start, wanted.StartOverridden, wanted.Ordinal
from @Scopes wanted
    inner join @newScopes newScope on newScope.Id = wanted.ScopeKey

update target set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,IsDeleted = 0
    ,GradeId = wanted.GradeId
    ,LessonStart = wanted.LessonStart
    ,LessonStartOverridden = wanted.LessonStartOverridden
    ,AssessmentStart = wanted.AssessmentStart
    ,AssessmentStartOverridden = wanted.AssessmentStartOverridden
from [Content].[CampaignSchedule] target
    inner join @Scopes wanted on wanted.ScopeKey = target.ActivationScopeId
where target.VersionOf = target.Id

insert [Content].[CampaignSchedule] (
     Id, VersionOf, Updated, UpdatedBy, ActivationScopeId, GradeId,
     LessonStart, LessonStartOverridden, AssessmentStart, AssessmentStartOverridden
)
select
     generated.Id, generated.Id, @now, @SessionId, wanted.ScopeKey, wanted.GradeId,
     wanted.LessonStart, wanted.LessonStartOverridden, wanted.AssessmentStart, wanted.AssessmentStartOverridden
from @Scopes wanted
    cross apply (values (newid())) generated(Id)
where not exists (
    select 1 from [Content].[CampaignSchedule-Active] existing where existing.ActivationScopeId = wanted.ScopeKey
)

update activation set Start = root.Start, Updated = @now, UpdatedBy = @SessionId
from [Content].[Activation] activation
    cross join (select Start from @Scopes where ParentScopeKey is null) root
where activation.Id = @ActivationId and activation.VersionOf = activation.Id

update stagePlan set
     ScopeLevel = case
         when exists (select 1 from @Scopes where OrganizationId <> DistrictOrganizationId) then 2
         when exists (select 1 from @Scopes where GradeId is not null) then 1
         else 0
     end
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ActivationStagePlan] stagePlan
where stagePlan.ActivationId = @ActivationId and stagePlan.VersionOf = stagePlan.Id

update target set
     Send = wanted.Send
    ,Overridden = wanted.Overridden
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[ActivationStage] target
    inner join [Content].[ActivationStagePlan-Active] stagePlan on stagePlan.Id = target.PlanId
    inner join @Schedules wanted on wanted.ScopeKey = target.ActivationScopeId and wanted.StageId = stagePlan.StageId
where stagePlan.ActivationId = @ActivationId and target.VersionOf = target.Id

insert [Content].[ActivationStage] (
     Id, VersionOf, Updated, UpdatedBy, PlanId, ActivationScopeId, Send, Overridden
)
select
     generated.Id, generated.Id, @now, @SessionId, stagePlan.Id, wanted.ScopeKey, wanted.Send, wanted.Overridden
from @Schedules wanted
    inner join [Content].[ActivationStagePlan-Active] stagePlan on stagePlan.ActivationId = @ActivationId and stagePlan.StageId = wanted.StageId
    cross apply (values (newid())) generated(Id)
where not exists (
    select 1 from [Content].[ActivationStage-Active] existing
    where existing.PlanId = stagePlan.Id and existing.ActivationScopeId = wanted.ScopeKey
)

-- Pending messages follow their revised stage occurrence.
update email set Send = activationStage.Send, Updated = @now, UpdatedBy = @SessionId
from [Content].[Email] email
    inner join [Content].[Message-Active] message on message.EmailId = email.Id
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.Id = message.ActivationStageId
    inner join [Content].[ActivationStagePlan-Active] stagePlan on stagePlan.Id = activationStage.PlanId
where stagePlan.ActivationId = @ActivationId and email.Status = 0 and email.VersionOf = email.Id

update sms set Send = activationStage.Send, Updated = @now, UpdatedBy = @SessionId
from [Content].[Sms] sms
    inner join [Content].[Message-Active] message on message.SmsId = sms.Id
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.Id = message.ActivationStageId
    inner join [Content].[ActivationStagePlan-Active] stagePlan on stagePlan.Id = activationStage.PlanId
where stagePlan.ActivationId = @ActivationId and sms.Status = 0 and sms.VersionOf = sms.Id

-- Realize messages and scoped memberships for newly added schedules.
declare @scopeId uniqueidentifier
declare @scopeOrganizationId uniqueidentifier
declare @scopeName nvarchar(150)
declare @stageId uniqueidentifier
declare @definitionId uniqueidentifier
declare @populationId uniqueidentifier
declare @messageTypeId uniqueidentifier
declare @emailTemplateId uniqueidentifier
declare @smsTemplateId uniqueidentifier
declare @activationStageId uniqueidentifier
declare @send datetimeoffset(7)
declare @membershipId uniqueidentifier
declare @messageId uniqueidentifier
declare @emailId uniqueidentifier
declare @smsId uniqueidentifier

declare newMessageCursor cursor local fast_forward for
select
     scope.ScopeKey, scope.OrganizationId, scope.Name, stage.Id, definition.Id,
     definition.PopulationId, definition.MessageTypeId, definition.EmailTemplateId,
     definition.SmsTemplateId, activationStage.Id, activationStage.Send
from @Scopes scope
    inner join @newScopes newScope on newScope.Id = scope.ScopeKey
    cross join [Content].[Stage-Active] stage
    inner join [Content].[Message-Active] definition on definition.StageId = stage.Id and definition.ActivationId is null
    inner join [Content].[ActivationStagePlan-Active] stagePlan on stagePlan.ActivationId = @ActivationId and stagePlan.StageId = stage.Id
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.PlanId = stagePlan.Id
        and activationStage.ActivationScopeId = scope.ScopeKey
where stage.CampaignId = @campaignId
order by scope.Ordinal, stage.Ordinal, definition.Ordinal

open newMessageCursor
fetch next from newMessageCursor into @scopeId, @scopeOrganizationId, @scopeName, @stageId,
    @definitionId, @populationId, @messageTypeId, @emailTemplateId, @smsTemplateId,
    @activationStageId, @send

while @@fetch_status = 0
begin
    set @membershipId = null
    select @membershipId = Id from [Content].[Membership-Active]
    where PopulationId = @populationId and OrganizationId = @scopeOrganizationId and ActivationScopeId = @scopeId

    if @membershipId is null
    begin
        set @membershipId = newid()
        insert [Content].[Membership] (
             Id, VersionOf, Updated, UpdatedBy, PortalId, Name, Description, SupportsOptOut,
             PopulationId, OrganizationId, ActivationScopeId
        )
        select
             @membershipId, @membershipId, @now, @SessionId, population.PortalId,
             left(population.Name + N' | ' + @scopeName, 75), population.Description,
             population.SupportsOptOut, population.Id, @scopeOrganizationId, @scopeId
        from [Content].[Population-Active] population
        where population.Id = @populationId and population.PortalId = @portalId
    end

    set @messageId = newid()
    set @emailId = null
    set @smsId = null

    if @messageTypeId = '893356f5-0baa-4b66-8b72-798285c6a4db'
    begin
        set @emailId = newid()
        insert [Content].[Email] (
             Id, VersionOf, Updated, UpdatedBy, MembershipId, FromName, FromEmail,
             TemplateId, Send, Subject, Body, Status, BatchId
        )
        select
             @emailId, @emailId, @now, @SessionId, @membershipId, @FromName, @FromEmail,
             template.Id, @send, template.Subject, template.Body, 0, null
        from [Content].[EmailTemplate-Active] template
        where template.Id = @emailTemplateId and template.PortalId = @portalId
            and (template.OrganizationId is null or template.OrganizationId in (@organizationId, @scopeOrganizationId))
    end
    else if @messageTypeId = '017e2c80-91ab-4090-bf7a-fe670ca4b180'
    begin
        set @smsId = newid()
        insert [Content].[Sms] (
             Id, VersionOf, Updated, UpdatedBy, MembershipId, TemplateId, Send, Body, Status, BatchId
        )
        select
             @smsId, @smsId, @now, @SessionId, @membershipId,
             template.Id, @send, template.Body, 0, null
        from [Content].[SmsTemplate-Active] template
        where template.Id = @smsTemplateId and template.PortalId = @portalId
            and (template.OrganizationId is null or template.OrganizationId in (@organizationId, @scopeOrganizationId))
    end

    insert [Content].[Message] (
         Id, VersionOf, Updated, UpdatedBy, StageId, ActivationId, DefinitionId,
         MembershipId, EmailId, SmsId, ActivationStageId
    ) values (
         @messageId, @messageId, @now, @SessionId, @stageId, @ActivationId, @definitionId,
         @membershipId, @emailId, @smsId, @activationStageId
    )

    fetch next from newMessageCursor into @scopeId, @scopeOrganizationId, @scopeName, @stageId,
        @definitionId, @populationId, @messageTypeId, @emailTemplateId, @smsTemplateId,
        @activationStageId, @send
end

close newMessageCursor
deallocate newMessageCursor

commit transaction