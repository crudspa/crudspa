create view [Education].[GuidePage-Active] as

select guidePage.Id as Id
    ,guidePage.GuideBinderId as GuideBinderId
    ,guidePage.PageId as PageId
    ,guidePage.ShowGuide as ShowGuide
    ,guidePage.GuideText as GuideText
    ,guidePage.GuideAudioId as GuideAudioId
    ,guidePage.ShowNotebook as ShowNotebook
from [Education].[GuidePage] guidePage
where 1=1
    and guidePage.IsDeleted = 0
    and guidePage.VersionOf = guidePage.Id