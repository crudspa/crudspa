create trigger [Content].[OptionsAnswerTrigger] on [Content].[OptionsAnswer]
    for update
as

insert [Content].[OptionsAnswer] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,QuestionId
    ,Kind
    ,Orientation
    ,AllowOther
    ,OtherLabel
    ,MinSelections
    ,MaxSelections
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
    ,deleted.Orientation
    ,deleted.AllowOther
    ,deleted.OtherLabel
    ,deleted.MinSelections
    ,deleted.MaxSelections
    ,deleted.Ordering
from deleted