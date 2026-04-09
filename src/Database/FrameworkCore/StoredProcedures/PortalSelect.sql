create proc [FrameworkCore].[PortalSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     portal.Id
    ,portal.[Key]
    ,portal.Title
    ,(select count(1) from [Framework].[Segment-Active] where PortalId = portal.Id and ParentId is null) as SegmentCount
from [Framework].[Portal-Active] portal
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where portal.Id = @Id
    and organization.Id = @organizationId

select
     portalFeature.Id
    ,portalFeature.PortalId
    ,portalFeature.[Key]
    ,portalFeature.Title
    ,portalFeature.IconId
    ,portalFeature.PermissionId
    ,icon.CssClass as IconCssClass
from [Framework].[PortalFeature-Active] portalFeature
    inner join [Framework].[Portal-Active] portal on portalFeature.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[Icon-Active] icon on portalFeature.IconId = icon.Id
where portal.Id = @Id
    and portal.OwnerId = @organizationId