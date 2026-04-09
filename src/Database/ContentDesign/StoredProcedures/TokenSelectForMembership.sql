create proc [ContentDesign].[TokenSelectForMembership] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     token.Id
    ,token.MembershipId
    ,token.[Key]
    ,token.Description
    ,token.Ordinal
from [Content].[Token-Active] token
    inner join [Content].[Membership-Active] membership on token.MembershipId = membership.Id
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where token.MembershipId = @MembershipId
    and organization.Id = @organizationId