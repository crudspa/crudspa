create view [Education].[GameSectionType-Active] as

select gameSectionType.Id as Id
    ,gameSectionType.Name as Name
    ,gameSectionType.Ordinal as Ordinal
from [Education].[GameSectionType] gameSectionType
where 1=1
    and gameSectionType.IsDeleted = 0