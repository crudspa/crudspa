create trigger [Content].[DateAnswerTrigger] on [Content].[DateAnswer]
    for update
as

insert [Content].[DateAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionId
    ,Kind
    ,Label
    ,DateMin
    ,DateMax
    ,TimeMin
    ,TimeMax
    ,DateTimeMin
    ,DateTimeMax
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
    ,deleted.DateMin
    ,deleted.DateMax
    ,deleted.TimeMin
    ,deleted.TimeMax
    ,deleted.DateTimeMin
    ,deleted.DateTimeMax
from deleted