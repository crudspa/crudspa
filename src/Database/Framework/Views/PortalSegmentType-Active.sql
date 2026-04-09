create view [Framework].[PortalSegmentType-Active] as

select portalSegmentType.Id as Id
    ,portalSegmentType.PortalId as PortalId
    ,portalSegmentType.TypeId as TypeId
from [Framework].[PortalSegmentType] portalSegmentType
where 1=1
    and portalSegmentType.IsDeleted = 0
    and portalSegmentType.VersionOf = portalSegmentType.Id