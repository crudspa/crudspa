create proc [ContentMessaging].[EmailDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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
)
    throw 51000, 'Email access denied.', 1

begin transaction

update message
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Message] message
    inner join [Content].[Message-Active] activeMessage on activeMessage.Id = message.Id
where activeMessage.EmailId = @Id

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Email] baseTable
    inner join [Content].[Email-Active] email on email.Id = baseTable.Id
    inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
where baseTable.Id = @Id

commit transaction