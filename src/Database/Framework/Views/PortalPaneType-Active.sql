create view [Framework].[PortalPaneType-Active] as

select portalPaneType.Id as Id
    ,portalPaneType.PortalId as PortalId
    ,portalPaneType.TypeId as TypeId
from [Framework].[PortalPaneType] portalPaneType
where 1=1
    and portalPaneType.IsDeleted = 0
    and portalPaneType.VersionOf = portalPaneType.Id