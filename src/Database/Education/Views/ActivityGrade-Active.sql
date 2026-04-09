create view [Education].[ActivityGrade-Active] as

select activityGrade.Id as Id
    ,activityGrade.ActivityId as ActivityId
    ,activityGrade.AssessmentTypeId as AssessmentTypeId
    ,activityGrade.AssessmentLevelId as AssessmentLevelId
    ,activityGrade.GradeId as GradeId
from [Education].[ActivityGrade] activityGrade
where 1=1
    and activityGrade.IsDeleted = 0
    and activityGrade.VersionOf = activityGrade.Id