create proc [EducationStudent].[ModuleSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Education].[ModuleViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,ModuleId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
     module.Id
    ,module.Title
    ,icon.CssClass as IconName
    ,module.BookId
    ,module.StatusId
    ,module.BinderId
    ,module.Ordinal
    ,book.Title as BookTitle
    ,guideBinder.Id as GuideBinderId
    ,guideBinder.BinderId as GuideBinderBinderId
    ,guideBinder.GuideImageId as GuideBinderGuideImageId
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
    ,status.Name as StatusName
    ,binderType.DisplayView as BinderDisplayView
from [Education].[Module-Active] module
    inner join [Education].[Book-Active] book on module.BookId = book.Id
    left join [Education].[GuideBinder-Active] guideBinder on module.BinderId = guideBinder.BinderId
    left join [Framework].[ImageFile-Active] guideImage on guideBinder.GuideImageId = guideImage.Id
    inner join [Framework].[ContentStatus-Active] status on module.StatusId = status.Id
    left join [Framework].[Icon-Active] icon on module.IconId = icon.Id
    inner join [Content].[Binder] binder on module.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
where module.Id = @Id
    and module.StatusId = @ContentStatusComplete

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
from [Education].[Module-Active] module
    inner join [Education].[GuideBinder-Active] guideBinder on module.BinderId = guideBinder.BinderId
    inner join [Education].[GuidePage-Active] guidePage on guideBinder.Id = guidePage.GuideBinderId
    left join [Framework].[AudioFile-Active] guideAudio on guidePage.GuideAudioId = guideAudio.Id
where module.Id = @Id
    and module.StatusId = @ContentStatusComplete