create view [Education].[StudentAssessment-Active] as

select studentAssessment.Id as Id
    ,studentAssessment.StudentId as StudentId
    ,studentAssessment.AssessmentTypeId as AssessmentTypeId
    ,studentAssessment.AssessmentLevelId as AssessmentLevelId
    ,studentAssessment.Score as Score
from [Education].[StudentAssessment] studentAssessment
where 1=1
    and studentAssessment.IsDeleted = 0
    and studentAssessment.VersionOf = studentAssessment.Id