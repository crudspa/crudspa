create trigger [Education].[GuidePageTrigger] on [Education].[GuidePage]
    for update
as

insert [Education].[GuidePage] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,GuideBinderId
    ,PageId
    ,ShowGuide
    ,GuideText
    ,GuideAudioId
    ,ShowNotebook
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.GuideBinderId
    ,deleted.PageId
    ,deleted.ShowGuide
    ,deleted.GuideText
    ,deleted.GuideAudioId
    ,deleted.ShowNotebook
from deleted