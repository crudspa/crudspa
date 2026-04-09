create trigger [Content].[PageTrigger] on [Content].[Page]
    for update
as

insert [Content].[Page] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BinderId
    ,TypeId
    ,Title
    ,BoxId
    ,StatusId
    ,ShowNotebook
    ,ShowGuide
    ,GuideText
    ,GuideAudioId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BinderId
    ,deleted.TypeId
    ,deleted.Title
    ,deleted.BoxId
    ,deleted.StatusId
    ,deleted.ShowNotebook
    ,deleted.ShowGuide
    ,deleted.GuideText
    ,deleted.GuideAudioId
    ,deleted.Ordinal
from deleted