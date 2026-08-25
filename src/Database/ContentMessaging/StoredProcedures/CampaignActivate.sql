create proc [ContentMessaging].[CampaignActivate] (
     @SessionId uniqueidentifier
    ,@BatchId uniqueidentifier
    ,@CampaignId uniqueidentifier
    ,@FromName nvarchar(150)
    ,@FromEmail nvarchar(75)
    ,@Scopes [ContentMessaging].[CampaignScopeList] readonly
    ,@Schedules [ContentMessaging].[ScopedStageScheduleList] readonly
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
declare @userId uniqueidentifier
declare @publisherOrganizationId uniqueidentifier
declare @portalId uniqueidentifier
declare @scheduledStatusId uniqueidentifier = 'a9b01002-7c15-4fb8-b25f-9f5629031002'
declare @stageCount int
declare @activationCount int = 0
declare @membershipCount int = 0
declare @messageCount int = 0
declare @emailCount int = 0
declare @smsCount int = 0

select
     @userId = userTable.Id
    ,@publisherOrganizationId = userTable.OrganizationId
from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
where session.Id = @SessionId

if @userId is null
    throw 50000, 'A signed-in user is required to activate a Campaign.', 1

select @portalId = campaign.PortalId
from [Content].[Campaign-Active] campaign
    inner join [Framework].[Portal-Active] portal on campaign.PortalId = portal.Id
where campaign.Id = @CampaignId
    and portal.OwnerId = @publisherOrganizationId

if @portalId is null
    throw 50000, 'The Campaign is unavailable or is not owned by this Organization.', 1

if (select count(*) from @Scopes where ParentScopeKey is null) <> 1
    throw 50000, 'Exactly one district-wide schedule is required.', 1

declare @districtOrganizationId uniqueidentifier = (
    select DistrictOrganizationId from @Scopes where ParentScopeKey is null
)

if not exists (
    select 1
    from [Education].[District-Active] district
        inner join [Education].[Publisher-Active] publisher on publisher.Id = district.PublisherId
    where district.OrganizationId = @districtOrganizationId
        and publisher.OrganizationId = @publisherOrganizationId
)
    throw 50000, 'The selected District is unavailable to this Publisher.', 1

if not exists (
    select 1
    from [Education].[District-Active] district
        inner join [Content].[CampaignLicense-Active] campaignLicense on campaignLicense.CampaignId = @CampaignId
        inner join [Framework].[License-Active] license on license.Id = campaignLicense.LicenseId
        inner join [Education].[DistrictLicense-Active] districtLicense on districtLicense.DistrictId = district.Id
            and districtLicense.LicenseId = campaignLicense.LicenseId
    where district.OrganizationId = @districtOrganizationId
)
    throw 50000, 'The selected District is not licensed for this Campaign.', 1

if exists (select 1 from @Scopes where DistrictOrganizationId <> @districtOrganizationId)
    throw 50000, 'A schedule scope belongs to a different District.', 1

if exists (
    select 1 from @Scopes scope
    where (scope.ParentScopeKey is null and (scope.GradeId is not null or scope.OrganizationId <> scope.DistrictOrganizationId))
       or (scope.ParentScopeKey is not null and scope.GradeId is null)
)
    throw 50000, 'The submitted schedule scope hierarchy is invalid.', 1

if exists (
    select 1
    from @Scopes scope
        left join [Framework].[Organization-Active] organization on organization.Id = scope.OrganizationId
        left join [Education].[Grade-Active] grade on grade.Id = scope.GradeId
    where organization.Id is null or (scope.GradeId is not null and grade.Id is null)
)
    throw 50000, 'A schedule references an unavailable Organization or Grade.', 1

if exists (
    select 1
    from @Scopes scope
    where scope.OrganizationId <> @districtOrganizationId
        and not exists (
            select 1
            from [Education].[School-Active] school
                inner join [Education].[District-Active] district on district.Id = school.DistrictId
            where school.OrganizationId = scope.OrganizationId
                and district.OrganizationId = @districtOrganizationId
        )
)
    throw 50000, 'A school schedule belongs to a different District.', 1

if exists (
    select 1 from @Scopes scope
    where scope.ParentScopeKey is not null
        and not exists (select 1 from @Scopes parent where parent.ScopeKey = scope.ParentScopeKey)
)
    throw 50000, 'A schedule references an unavailable parent scope.', 1

if exists (
    select 1 from @Scopes
    where ParentScopeKey is not null
    group by OrganizationId, GradeId
    having count(*) > 1
)
    throw 50000, 'A grade or school schedule is duplicated.', 1

if exists (
    select 1
    from @Scopes scope
        cross join (select ScopeKey from @Scopes where ParentScopeKey is null) root
        outer apply (
            select ScopeKey
            from @Scopes grade
            where grade.OrganizationId = @districtOrganizationId
                and grade.GradeId = scope.GradeId
                and grade.ParentScopeKey is not null
        ) grade
    where scope.ParentScopeKey is not null
        and (
            (scope.OrganizationId = @districtOrganizationId and scope.ParentScopeKey <> root.ScopeKey)
            or
            (scope.OrganizationId <> @districtOrganizationId
                and scope.ParentScopeKey <> coalesce(grade.ScopeKey, root.ScopeKey))
        )
)
    throw 50000, 'A grade or school schedule has an invalid parent schedule.', 1

select @stageCount = count(*)
from [Content].[Stage-Active]
where CampaignId = @CampaignId

if @stageCount = 0 or @stageCount * (select count(*) from @Scopes) <> (select count(*) from @Schedules)
    throw 50000, 'The submitted schedule does not match the Campaign stages and scopes.', 1

if exists (
    select 1
    from @Schedules schedule
        inner join @Scopes scope on scope.ScopeKey = schedule.ScopeKey
        left join [Content].[Stage-Active] stage on stage.Id = schedule.StageId and stage.CampaignId = @CampaignId
    where stage.Id is null
)
    throw 50000, 'The submitted schedule contains an invalid Stage.', 1

if exists (
    select 1 from [Content].[Stage-Active] stage
    where stage.CampaignId = @CampaignId
        and not exists (
            select 1 from [Content].[Message-Active] message
            where message.StageId = stage.Id and message.ActivationId is null
        )
)
    throw 50000, 'Every Campaign Stage requires at least one Message.', 1

if exists (select 1 from [Content].[Activation-Active] where BatchId = @BatchId)
begin
    select @BatchId as BatchId, 0 as Activations, 0 as MembershipsCreated, 0 as Messages, 0 as Emails, 0 as Sms
    return
end

begin transaction

declare @scopeIds table (
     ScopeKey uniqueidentifier primary key
    ,ScopeId uniqueidentifier not null
    ,ActivationId uniqueidentifier not null
)

declare @rootKey uniqueidentifier
declare @activationId uniqueidentifier
declare @rootStart date

declare rootCursor cursor local fast_forward for
select ScopeKey, DistrictOrganizationId, Start
from @Scopes
where ParentScopeKey is null
order by DistrictOrganizationId

open rootCursor
fetch next from rootCursor into @rootKey, @districtOrganizationId, @rootStart

while @@fetch_status = 0
begin
    set @activationId = null
    select @activationId = activation.Id
    from [Content].[Activation-Active] activation with (updlock, holdlock)
    where activation.BatchId = @BatchId
        and activation.OrganizationId = @districtOrganizationId

    if @activationId is null
    begin
        set @activationId = newid()
        insert [Content].[Activation] (
             Id, VersionOf, Updated, UpdatedBy, BatchId, CampaignId, OrganizationId,
             Start, Activated, ActivatedBy, StatusId
        ) values (
             @activationId, @activationId, @now, @SessionId, @BatchId, @CampaignId, @districtOrganizationId,
             @rootStart, @now, @userId, @scheduledStatusId
        )
        set @activationCount += 1
    end

    insert @scopeIds (ScopeKey, ScopeId, ActivationId)
    values (@rootKey, newid(), @activationId)

    fetch next from rootCursor into @rootKey, @districtOrganizationId, @rootStart
end

close rootCursor
deallocate rootCursor

insert @scopeIds (ScopeKey, ScopeId, ActivationId)
select scope.ScopeKey, newid(), root.ActivationId
from @Scopes scope
    inner join @scopeIds root on root.ScopeKey = (
        select top 1 ancestor.ScopeKey
        from @Scopes ancestor
        where ancestor.DistrictOrganizationId = scope.DistrictOrganizationId
            and ancestor.ParentScopeKey is null
    )
where scope.ParentScopeKey is not null

insert [Content].[ActivationScope] (
     Id, VersionOf, Updated, UpdatedBy, ActivationId, ParentId, OrganizationId,
     Name, Start, StartOverridden, Ordinal
)
select
     ids.ScopeId, ids.ScopeId, @now, @SessionId, ids.ActivationId, parentIds.ScopeId, scope.OrganizationId,
     scope.Name, scope.Start, scope.StartOverridden, scope.Ordinal
from @Scopes scope
    inner join @scopeIds ids on ids.ScopeKey = scope.ScopeKey
    left join @scopeIds parentIds on parentIds.ScopeKey = scope.ParentScopeKey
where not exists (select 1 from [Content].[ActivationScope] existing where existing.Id = ids.ScopeId)

insert [Content].[CampaignSchedule] (
     Id, VersionOf, Updated, UpdatedBy, ActivationScopeId, GradeId,
     LessonStart, LessonStartOverridden, AssessmentStart, AssessmentStartOverridden
)
select
     generated.Id, generated.Id, @now, @SessionId, ids.ScopeId, scope.GradeId,
     scope.LessonStart, scope.LessonStartOverridden, scope.AssessmentStart, scope.AssessmentStartOverridden
from @Scopes scope
    inner join @scopeIds ids on ids.ScopeKey = scope.ScopeKey
    cross apply (values (newid())) generated(Id)
where not exists (
    select 1 from [Content].[CampaignSchedule-Active] existing where existing.ActivationScopeId = ids.ScopeId
)

declare @plans table (
     ActivationId uniqueidentifier not null
    ,StageId uniqueidentifier not null
    ,PlanId uniqueidentifier not null
    ,primary key (ActivationId, StageId)
)

insert @plans (ActivationId, StageId, PlanId)
select activation.ActivationId, stage.Id, newid()
from (select distinct ActivationId from @scopeIds) activation
    cross join [Content].[Stage-Active] stage
where stage.CampaignId = @CampaignId

insert [Content].[ActivationStagePlan] (
     Id, VersionOf, Updated, UpdatedBy, ActivationId, StageId, ScopeLevel
)
select
     stagePlan.PlanId, stagePlan.PlanId, @now, @SessionId, stagePlan.ActivationId, stagePlan.StageId,
     case
         when exists (
             select 1 from @Scopes scope inner join @scopeIds ids on ids.ScopeKey = scope.ScopeKey
             where ids.ActivationId = stagePlan.ActivationId and scope.OrganizationId <> scope.DistrictOrganizationId
         ) then 2
         when exists (
             select 1 from @Scopes scope inner join @scopeIds ids on ids.ScopeKey = scope.ScopeKey
             where ids.ActivationId = stagePlan.ActivationId and scope.GradeId is not null
         ) then 1
         else 0
     end
from @plans stagePlan
where not exists (
    select 1 from [Content].[ActivationStagePlan-Active] existing
    where existing.ActivationId = stagePlan.ActivationId and existing.StageId = stagePlan.StageId
)

insert [Content].[ActivationStage] (
     Id, VersionOf, Updated, UpdatedBy, PlanId, ActivationScopeId, Send, Overridden
)
select
     generated.Id, generated.Id, @now, @SessionId, stagePlan.PlanId, ids.ScopeId, schedule.Send, schedule.Overridden
from @Schedules schedule
    inner join @scopeIds ids on ids.ScopeKey = schedule.ScopeKey
    inner join @plans stagePlan on stagePlan.ActivationId = ids.ActivationId and stagePlan.StageId = schedule.StageId
    cross apply (values (newid())) generated(Id)

declare @definitionId uniqueidentifier
declare @stageId uniqueidentifier
declare @populationId uniqueidentifier
declare @messageTypeId uniqueidentifier
declare @emailTemplateId uniqueidentifier
declare @smsTemplateId uniqueidentifier
declare @scopeId uniqueidentifier
declare @scopeName nvarchar(150)
declare @scopeOrganizationId uniqueidentifier
declare @scopeDistrictOrganizationId uniqueidentifier
declare @activationStageId uniqueidentifier
declare @send datetimeoffset(7)
declare @membershipId uniqueidentifier
declare @messageId uniqueidentifier
declare @emailId uniqueidentifier
declare @smsId uniqueidentifier

declare messageCursor cursor local fast_forward for
select
     message.Id, stage.Id, message.PopulationId, message.MessageTypeId,
     message.EmailTemplateId, message.SmsTemplateId, ids.ScopeId, scope.Name,
     scope.OrganizationId, scope.DistrictOrganizationId, activationStage.Id, activationStage.Send
from [Content].[Stage-Active] stage
    inner join [Content].[Message-Active] message on message.StageId = stage.Id and message.ActivationId is null
    inner join @plans stagePlan on stagePlan.StageId = stage.Id
    inner join @scopeIds ids on ids.ActivationId = stagePlan.ActivationId
    inner join @Scopes scope on scope.ScopeKey = ids.ScopeKey
    inner join [Content].[ActivationStage-Active] activationStage on activationStage.PlanId = stagePlan.PlanId
        and activationStage.ActivationScopeId = ids.ScopeId
where stage.CampaignId = @CampaignId
order by ids.ActivationId, scope.Ordinal, stage.Ordinal, message.Ordinal

open messageCursor
fetch next from messageCursor into @definitionId, @stageId, @populationId, @messageTypeId,
    @emailTemplateId, @smsTemplateId, @scopeId, @scopeName, @scopeOrganizationId,
    @scopeDistrictOrganizationId, @activationStageId, @send

while @@fetch_status = 0
begin
    set @membershipId = null
    select @membershipId = membership.Id
    from [Content].[Membership-Active] membership with (updlock, holdlock)
    where membership.PopulationId = @populationId
        and membership.OrganizationId = @scopeOrganizationId
        and membership.ActivationScopeId = @scopeId

    if @membershipId is null
    begin
        set @membershipId = newid()
        insert [Content].[Membership] (
             Id, VersionOf, Updated, UpdatedBy, PortalId, Name, Description,
             SupportsOptOut, PopulationId, OrganizationId, ActivationScopeId
        )
        select
             @membershipId, @membershipId, @now, @SessionId, population.PortalId,
             left(population.Name + N' | ' + @scopeName, 75), population.Description,
             population.SupportsOptOut, population.Id, @scopeOrganizationId, @scopeId
        from [Content].[Population-Active] population
        where population.Id = @populationId and population.PortalId = @portalId

        if @@rowcount <> 1
            throw 50000, 'A Stage Population is unavailable for this Portal.', 1
        set @membershipCount += 1
    end

    set @messageId = newid()
    set @emailId = null
    set @smsId = null

    if @messageTypeId = '893356f5-0baa-4b66-8b72-798285c6a4db'
    begin
        set @emailId = newid()
        insert [Content].[Email] (
             Id, VersionOf, Updated, UpdatedBy, MembershipId, FromName,
             FromEmail, TemplateId, Send, Subject, Body, Status, BatchId
        )
        select
             @emailId, @emailId, @now, @SessionId, @membershipId, @FromName,
             @FromEmail, template.Id, @send, template.Subject, template.Body, 0, null
        from [Content].[EmailTemplate-Active] template
        where template.Id = @emailTemplateId and template.PortalId = @portalId
            and (template.OrganizationId is null or template.OrganizationId in (@scopeOrganizationId, @scopeDistrictOrganizationId))

        if @@rowcount <> 1
            throw 50000, 'An Email Stage has an unavailable template.', 1
        set @emailCount += 1
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
            and (template.OrganizationId is null or template.OrganizationId in (@scopeOrganizationId, @scopeDistrictOrganizationId))

        if @@rowcount <> 1
            throw 50000, 'An SMS Stage has an unavailable template.', 1
        set @smsCount += 1
    end
    else
        throw 50000, 'A Stage has an unsupported message type.', 1

    insert [Content].[Message] (
         Id, VersionOf, Updated, UpdatedBy, ActivationId, StageId, DefinitionId,
         MembershipId, EmailId, SmsId, ActivationStageId
    )
    select
         @messageId, @messageId, @now, @SessionId, stagePlan.ActivationId, @stageId, @definitionId,
         @membershipId, @emailId, @smsId, @activationStageId
    from @plans stagePlan where stagePlan.PlanId = (
        select PlanId from [Content].[ActivationStage-Active] where Id = @activationStageId
    )

    set @messageCount += 1

    fetch next from messageCursor into @definitionId, @stageId, @populationId, @messageTypeId,
        @emailTemplateId, @smsTemplateId, @scopeId, @scopeName, @scopeOrganizationId,
        @scopeDistrictOrganizationId, @activationStageId, @send
end

close messageCursor
deallocate messageCursor

commit transaction

select
     @BatchId as BatchId
    ,@activationCount as Activations
    ,@membershipCount as MembershipsCreated
    ,@messageCount as Messages
    ,@emailCount as Emails
    ,@smsCount as Sms