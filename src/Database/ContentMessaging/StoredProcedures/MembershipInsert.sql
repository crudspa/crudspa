create proc [ContentMessaging].[MembershipInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Name nvarchar(75)
    ,@Description nvarchar(max)
    ,@SupportsOptOut bit
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @portalOwnerId uniqueidentifier = (select OwnerId from [Framework].[Portal-Active] where Id = @PortalId)
declare @membershipOrganizationId uniqueidentifier = case when @organizationId = @portalOwnerId then null else @organizationId end

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, @PortalId, @membershipOrganizationId))
    throw 51000, 'Membership access denied.', 1

begin transaction

insert [Content].[Membership] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,Name
    ,Description
    ,SupportsOptOut
    ,OrganizationId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@Name
    ,@Description
    ,@SupportsOptOut
    ,@membershipOrganizationId
)

commit transaction