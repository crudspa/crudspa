create proc [ContentMessaging].[EmailUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@FromName nvarchar(150)
    ,@FromEmail nvarchar(75)
    ,@TemplateId uniqueidentifier
    ,@Send datetimeoffset(7)
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
    from [Content].[Email-Active] email
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where email.Id = @Id
        and email.Status = 0
        and (@TemplateId is null or exists (
            select 1 from [Content].[EmailTemplate-Active] template
            where template.Id = @TemplateId and template.PortalId = membership.PortalId
                and (template.OrganizationId is null or template.OrganizationId = membership.OrganizationId)
        ))
)
    throw 51000, 'Email access denied.', 1

begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,FromName = @FromName
    ,FromEmail = @FromEmail
    ,TemplateId = @TemplateId
    ,Send = @Send
    ,Subject = @Subject
    ,Body = @Body
from [Content].[Email] baseTable
    inner join [Content].[Email-Active] email on email.Id = baseTable.Id
    inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
where baseTable.Id = @Id

commit transaction