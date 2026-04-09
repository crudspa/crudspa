create trigger [Education].[ActivityGradeTrigger] on [Education].[ActivityGrade]
    for update
as

insert [Education].[ActivityGrade] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ActivityId
    ,AssessmentTypeId
    ,AssessmentLevelId
    ,GradeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ActivityId
    ,deleted.AssessmentTypeId
    ,deleted.AssessmentLevelId
    ,deleted.GradeId
from deleted