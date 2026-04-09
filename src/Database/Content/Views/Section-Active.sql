create view [Content].[Section-Active] as

select section.Id as Id
    ,section.PageId as PageId
    ,section.TypeId as TypeId
    ,section.BoxId as BoxId
    ,section.ContainerId as ContainerId
    ,section.Ordinal as Ordinal
from [Content].[Section] section
where 1=1
    and section.IsDeleted = 0
    and section.VersionOf = section.Id