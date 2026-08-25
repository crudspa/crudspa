create proc [ContentMessaging].[EmailTemplateUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@Subject nvarchar(150)
    ,@Body nvarchar(max)
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[EmailTemplate-Active] template
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, template.PortalId, template.OrganizationId)
    where template.Id = @Id
)
    throw 51000, 'Email template access denied.', 1

begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,Subject = @Subject
    ,Body = @Body
from [Content].[EmailTemplate] baseTable
    inner join [Content].[EmailTemplate-Active] emailTemplate on emailTemplate.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction