create proc [ContentMessaging].[EmailInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@ActivationId uniqueidentifier
    ,@StageId uniqueidentifier
    ,@FromName nvarchar(150)
    ,@FromEmail nvarchar(75)
    ,@TemplateId uniqueidentifier
    ,@Send datetimeoffset(7)
    ,@Subject nvarchar(150)
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[Membership-Active] membership
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where membership.Id = @MembershipId
        and (@TemplateId is null or exists (
            select 1 from [Content].[EmailTemplate-Active] template
            where template.Id = @TemplateId and template.PortalId = membership.PortalId
                and (template.OrganizationId is null or template.OrganizationId = membership.OrganizationId)
        ))
)
    throw 51000, 'Email access denied.', 1

if (@ActivationId is null and @StageId is not null)
    or (@ActivationId is not null and @StageId is null)
    throw 51000, 'Activation and Stage must be provided together.', 1

declare @activationStageId uniqueidentifier

if @ActivationId is not null
begin
    select @activationStageId = activationStage.Id
    from [Content].[Membership-Active] membership
        inner join [Content].[ActivationScope-Active] scope on scope.Id = membership.ActivationScopeId
        inner join [Content].[Activation-Active] activation on activation.Id = scope.ActivationId
        inner join [Content].[Campaign-Active] campaign on campaign.Id = activation.CampaignId
        inner join [Content].[ActivationStagePlan-Active] stagePlan on stagePlan.ActivationId = activation.Id
            and stagePlan.StageId = @StageId
        inner join [Content].[ActivationStage-Active] activationStage on activationStage.PlanId = stagePlan.Id
            and activationStage.ActivationScopeId = scope.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, campaign.PortalId, activation.OrganizationId)
    where membership.Id = @MembershipId
        and activation.Id = @ActivationId

    if @activationStageId is null
        throw 51000, 'Email Activation context is invalid.', 1
end

begin transaction

insert [Content].[Email] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MembershipId
    ,FromName
    ,FromEmail
    ,TemplateId
    ,Send
    ,Subject
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MembershipId
    ,@FromName
    ,@FromEmail
    ,@TemplateId
    ,@Send
    ,@Subject
    ,@Body
)

if @ActivationId is not null
begin
    declare @messageId uniqueidentifier = newid()

    insert [Content].[Message] (
         Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,StageId
        ,ActivationId
        ,MembershipId
        ,EmailId
        ,ActivationStageId
    ) values (
         @messageId
        ,@messageId
        ,@now
        ,@SessionId
        ,@StageId
        ,@ActivationId
        ,@MembershipId
        ,@Id
        ,@activationStageId
    )
end

commit transaction