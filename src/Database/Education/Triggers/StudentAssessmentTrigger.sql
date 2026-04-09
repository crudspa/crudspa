create trigger [Education].[StudentAssessmentTrigger] on [Education].[StudentAssessment]
    for update
as

insert [Education].[StudentAssessment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,StudentId
    ,AssessmentTypeId
    ,AssessmentLevelId
    ,Score
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.StudentId
    ,deleted.AssessmentTypeId
    ,deleted.AssessmentLevelId
    ,deleted.Score
from deleted