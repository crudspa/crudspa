create proc [EducationCommon].[ResearchGroupSelectNames] as
select
    researchGroup.Id
    ,researchGroup.[Name] as Name
    ,researchGroup.Ordinal as Ordinal
from [Education].[ResearchGroup-Active] as researchGroup
order by researchGroup.Ordinal