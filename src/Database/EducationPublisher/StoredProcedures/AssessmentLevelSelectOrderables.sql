create proc [EducationPublisher].[AssessmentLevelSelectOrderables] as

set nocount on
select
     assessmentLevel.Id
    ,assessmentLevel.Name as Name
    ,assessmentLevel.Ordinal
from [Education].[AssessmentLevel-Active] assessmentLevel
order by assessmentLevel.Ordinal