create trigger [Content].[ScaleAnswerTrigger] on [Content].[ScaleAnswer]
    for update
as

insert [Content].[ScaleAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionId
    ,Kind
    ,RatingKind
    ,LikertKind
    ,RatingMin
    ,RatingMax
    ,Ordering
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.QuestionId
    ,deleted.Kind
    ,deleted.RatingKind
    ,deleted.LikertKind
    ,deleted.RatingMin
    ,deleted.RatingMax
    ,deleted.Ordering
from deleted