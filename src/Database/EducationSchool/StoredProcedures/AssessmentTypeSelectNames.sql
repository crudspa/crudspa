create proc [EducationSchool].[AssessmentTypeSelectNames] as
select
    assessmentType.Id
    ,assessmentType.[Name] as Name
    ,assessmentType.Ordinal as Ordinal
from [Education].[AssessmentType-Active] as assessmentType
order by assessmentType.Ordinal