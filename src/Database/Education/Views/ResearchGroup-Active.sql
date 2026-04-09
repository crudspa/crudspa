create view [Education].[ResearchGroup-Active] as

select researchGroup.Id as Id
    ,researchGroup.Name as Name
    ,researchGroup.Ordinal as Ordinal
from [Education].[ResearchGroup] researchGroup
where 1=1
    and researchGroup.IsDeleted = 0