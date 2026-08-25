create proc [ContentMessaging].[SmsTemplateUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@Body nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[SmsTemplate-Active] template
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, template.PortalId, template.OrganizationId)
    where template.Id = @Id
)
    throw 51000, 'SMS template access denied.', 1

begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,Body = @Body
from [Content].[SmsTemplate] baseTable
    inner join [Content].[SmsTemplate-Active] smsTemplate on smsTemplate.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction