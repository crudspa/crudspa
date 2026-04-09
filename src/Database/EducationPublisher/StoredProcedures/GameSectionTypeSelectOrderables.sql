create proc [EducationPublisher].[GameSectionTypeSelectOrderables] as

set nocount on
select
     gameSectionType.Id
    ,gameSectionType.Name as Name
    ,gameSectionType.Ordinal
from [Education].[GameSectionType-Active] gameSectionType
order by gameSectionType.Ordinal