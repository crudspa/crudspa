create trigger [Content].[FileAnswerTrigger] on [Content].[FileAnswer]
    for update
as

insert [Content].[FileAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionId
    ,Kind
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.QuestionId
    ,deleted.Kind
from deleted