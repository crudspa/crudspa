create proc [EducationSchool].[AssessmentLevelSelectNames] as
select
    assessmentLevel.Id
    ,assessmentLevel.[Name] as Name
    ,assessmentLevel.Ordinal as Ordinal
from [Education].[AssessmentLevel-Active] as assessmentLevel
where Id != 'e35d7019-2361-429b-8846-e95eb45ccf3b'
order by assessmentLevel.Ordinal