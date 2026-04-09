create proc [ContentDesign].[EmailUpdate] (
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
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction