create proc [ContentMessaging].[MembershipUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(75)
    ,@Description nvarchar(max)
    ,@SupportsOptOut bit
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
    from [Content].[Membership-Active] membership
        cross apply [ContentMessaging].[SessionCanWriteOrganization](@SessionId, membership.PortalId, membership.OrganizationId)
    where membership.Id = @Id
)
    throw 51000, 'Membership access denied.', 1

begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,Description = @Description
    ,SupportsOptOut = @SupportsOptOut
from [Content].[Membership] baseTable
    inner join [Content].[Membership-Active] membership on membership.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction