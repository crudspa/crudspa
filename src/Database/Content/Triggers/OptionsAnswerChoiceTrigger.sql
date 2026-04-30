create trigger [Content].[OptionsAnswerChoiceTrigger] on [Content].[OptionsAnswerChoice]
    for update
as

insert [Content].[OptionsAnswerChoice] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OptionsAnswerId
    ,Text
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OptionsAnswerId
    ,deleted.Text
    ,deleted.Ordinal
from deleted