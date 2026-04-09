create trigger [Education].[AppSurveyResponseTrigger] on [Education].[AppSurveyResponse]
    for update
as

insert [Education].[AppSurveyResponse] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,AssignmentBatchId
    ,QuestionId
    ,AnswerId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.AssignmentBatchId
    ,deleted.QuestionId
    ,deleted.AnswerId
from deleted