create proc [EducationPublisher].[GuidePageSelectForPage] (
     @SessionId uniqueidentifier
    ,@PageId uniqueidentifier
) as

set nocount on
select
     guidePage.Id
    ,guidePage.GuideBinderId
    ,guidePage.PageId
    ,guidePage.ShowGuide
    ,guidePage.GuideText
    ,guidePage.GuideAudioId
    ,guideAudio.Id as GuideAudioId
    ,guideAudio.BlobId as GuideAudioBlobId
    ,guideAudio.Name as GuideAudioName
    ,guideAudio.Format as GuideAudioFormat
    ,guideAudio.OptimizedStatus as GuideAudioOptimizedStatus
    ,guideAudio.OptimizedBlobId as GuideAudioOptimizedBlobId
    ,guideAudio.OptimizedFormat as GuideAudioOptimizedFormat
    ,guidePage.ShowNotebook
from [Education].[GuidePage-Active] guidePage
    left join [Framework].[AudioFile-Active] guideAudio on guidePage.GuideAudioId = guideAudio.Id
where guidePage.PageId = @PageId