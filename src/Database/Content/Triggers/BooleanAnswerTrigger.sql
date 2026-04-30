create trigger [Content].[BooleanAnswerTrigger] on [Content].[BooleanAnswer]
    for update
as

insert [Content].[BooleanAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionId
    ,Kind
    ,[Default]
    ,Orientation
    ,TrueLabel
    ,FalseLabel
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.QuestionId
    ,deleted.Kind
    ,deleted.[Default]
    ,deleted.Orientation
    ,deleted.TrueLabel
    ,deleted.FalseLabel
from deleted