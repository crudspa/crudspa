create proc [ContentDesign].[MembershipSelectForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     membership.Id
    ,membership.PortalId
    ,membership.Name
    ,membership.Description
    ,membership.SupportsOptOut
    ,(select count(1) from [Content].[Member-Active] where MembershipId = membership.Id) as MemberCount
    ,(select count(1) from [Content].[Email-Active] where MembershipId = membership.Id) as EmailCount
    ,(select count(1) from [Content].[EmailTemplate-Active] where MembershipId = membership.Id) as EmailTemplateCount
    ,(select count(1) from [Content].[Token-Active] where MembershipId = membership.Id) as TokenCount
from [Content].[Membership-Active] membership
    inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where membership.PortalId = @PortalId
    and organization.Id = @organizationId