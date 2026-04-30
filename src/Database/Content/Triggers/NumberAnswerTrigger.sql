create trigger [Content].[NumberAnswerTrigger] on [Content].[NumberAnswer]
    for update
as

insert [Content].[NumberAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionId
    ,Kind
    ,IntegerMin
    ,IntegerMax
    ,DecimalMin
    ,DecimalMax
    ,CurrencyMin
    ,CurrencyMax
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.QuestionId
    ,deleted.Kind
    ,deleted.IntegerMin
    ,deleted.IntegerMax
    ,deleted.DecimalMin
    ,deleted.DecimalMax
    ,deleted.CurrencyMin
    ,deleted.CurrencyMax
from deleted