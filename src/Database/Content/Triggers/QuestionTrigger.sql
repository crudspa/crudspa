create trigger [Content].[QuestionTrigger] on [Content].[Question]
    for update
as

insert [Content].[Question] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Text
    ,AnswerTypeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Text
    ,deleted.AnswerTypeId
from deleted