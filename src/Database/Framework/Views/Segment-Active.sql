create view [Framework].[Segment-Active] as

select segment.Id as Id
    ,segment.[Key] as [Key]
    ,segment.Title as Title
    ,segment.StatusId as StatusId
    ,segment.Fixed as Fixed
    ,segment.RequiresId as RequiresId
    ,segment.PortalId as PortalId
    ,segment.TypeId as TypeId
    ,segment.PermissionId as PermissionId
    ,segment.IconId as IconId
    ,segment.ParentId as ParentId
    ,segment.Recursive as Recursive
    ,segment.Vertical as Vertical
    ,segment.ConfigJson as ConfigJson
    ,segment.AllLicenses as AllLicenses
    ,segment.Ordinal as Ordinal
from [Framework].[Segment] segment
where 1=1
    and segment.IsDeleted = 0
    and segment.VersionOf = segment.Id