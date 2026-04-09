create view [Framework].[PortalFeature-Active] as

select portalFeature.Id as Id
    ,portalFeature.PortalId as PortalId
    ,portalFeature.[Key] as [Key]
    ,portalFeature.Title as Title
    ,portalFeature.IconId as IconId
    ,portalFeature.PermissionId as PermissionId
from [Framework].[PortalFeature] portalFeature
where 1=1