create view [Content].[SectionType-Active] as

select sectionType.Id as Id
    ,sectionType.Name as Name
    ,sectionType.DesignView as DesignView
    ,sectionType.DisplayView as DisplayView
    ,sectionType.Ordinal as Ordinal
from [Content].[SectionType] sectionType
where 1=1
    and sectionType.IsDeleted = 0