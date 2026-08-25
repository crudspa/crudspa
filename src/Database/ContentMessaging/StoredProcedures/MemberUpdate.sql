create proc [ContentMessaging].[MemberUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Status int
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
    from [Content].[Member-Active] member
        inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where member.Id = @Id
)
    throw 51000, 'Member access denied.', 1

begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Status = @Status
from [Content].[Member] baseTable
    inner join [Content].[Member-Active] member on member.Id = baseTable.Id
    inner join [Content].[Membership-Active] membership on member.MembershipId = membership.Id
where baseTable.Id = @Id

commit transaction