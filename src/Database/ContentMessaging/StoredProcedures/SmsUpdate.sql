create proc [ContentMessaging].[SmsUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@TemplateId uniqueidentifier
    ,@Send datetimeoffset(7)
    ,@Body nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[Sms-Active] sms
        inner join [Content].[Membership-Active] membership on sms.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where sms.Id = @Id
        and (@TemplateId is null or exists (
            select 1 from [Content].[SmsTemplate-Active] template
            where template.Id = @TemplateId and template.PortalId = membership.PortalId
                and (template.OrganizationId is null or template.OrganizationId = membership.OrganizationId)
        ))
)
    throw 51000, 'SMS access denied.', 1

begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TemplateId = @TemplateId
    ,Send = @Send
    ,Body = @Body
from [Content].[Sms] baseTable
    inner join [Content].[Sms-Active] sms on sms.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction