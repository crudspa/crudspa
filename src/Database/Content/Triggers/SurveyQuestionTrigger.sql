create trigger [Content].[SurveyQuestionTrigger] on [Content].[SurveyQuestion]
    for update
as

insert [Content].[SurveyQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PartId
    ,QuestionId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PartId
    ,deleted.QuestionId
    ,deleted.Ordinal
from deleted