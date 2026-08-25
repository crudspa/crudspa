create proc [ContentMessaging].[SmsInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@TemplateId uniqueidentifier
    ,@Send datetimeoffset(7)
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

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
            select 1 from [Content].[SmsTemplate-Active] template
            where template.Id = @TemplateId and template.PortalId = membership.PortalId
                and (template.OrganizationId is null or template.OrganizationId = membership.OrganizationId)
        ))
)
    throw 51000, 'SMS access denied.', 1

begin transaction

insert [Content].[Sms] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MembershipId
    ,TemplateId
    ,Send
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MembershipId
    ,@TemplateId
    ,@Send
    ,@Body
)

commit transaction