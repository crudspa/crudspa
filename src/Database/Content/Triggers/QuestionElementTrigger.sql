create trigger [Content].[QuestionElementTrigger] on [Content].[QuestionElement]
    for update
as

insert [Content].[QuestionElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,QuestionId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.QuestionId
from deleted