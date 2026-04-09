create view [Content].[Page-Active] as

select page.Id as Id
    ,page.BinderId as BinderId
    ,page.TypeId as TypeId
    ,page.Title as Title
    ,page.BoxId as BoxId
    ,page.StatusId as StatusId
    ,page.ShowNotebook as ShowNotebook
    ,page.ShowGuide as ShowGuide
    ,page.GuideText as GuideText
    ,page.GuideAudioId as GuideAudioId
    ,page.Ordinal as Ordinal
from [Content].[Page] page
where 1=1
    and page.IsDeleted = 0
    and page.VersionOf = page.Id