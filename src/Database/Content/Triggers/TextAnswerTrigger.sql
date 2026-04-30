create trigger [Content].[TextAnswerTrigger] on [Content].[TextAnswer]
    for update
as

insert [Content].[TextAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionId
    ,Kind
    ,Label
    ,Placeholder
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.QuestionId
    ,deleted.Kind
    ,deleted.Label
    ,deleted.Placeholder
from deleted