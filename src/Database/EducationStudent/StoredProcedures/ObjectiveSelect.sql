create proc [EducationStudent].[ObjectiveSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Education].[ObjectiveViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,ObjectiveId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
     objective.Id
    ,objective.Title
    ,objective.StatusId
    ,objective.LessonId
    ,objective.TrophyImageId
    ,objective.BinderId
    ,objective.Ordinal
    ,lesson.Title as LessonTitle
    ,lesson.UnitId as LessonUnitId
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
    ,lessonUnit.Title as LessonUnitTitle
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
    ,binderType.DisplayView as BinderTypeDisplayView
from [Education].[Objective-Active] objective
    inner join [Education].[Lesson-Active] lesson on objective.LessonId = lesson.Id
    left join [Education].[GuideBinder-Active] guideBinder on objective.BinderId = guideBinder.BinderId
    left join [Framework].[ImageFile-Active] guideImage on guideBinder.GuideImageId = guideImage.Id
    inner join [Education].[Unit-Active] lessonUnit on lesson.UnitId = lessonUnit.Id
    left join [Framework].[ImageFile-Active] trophyImage on objective.TrophyImageId = trophyImage.Id
    inner join [Content].[Binder] binder on objective.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
where objective.Id = @Id
    and objective.StatusId = @ContentStatusComplete

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
from [Education].[Objective-Active] objective
    inner join [Education].[GuideBinder-Active] guideBinder on objective.BinderId = guideBinder.BinderId
    inner join [Education].[GuidePage-Active] guidePage on guideBinder.Id = guidePage.GuideBinderId
    left join [Framework].[AudioFile-Active] guideAudio on guidePage.GuideAudioId = guideAudio.Id
where objective.Id = @Id
    and objective.StatusId = @ContentStatusComplete