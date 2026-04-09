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
    ,lessonGuideImage.Id as LessonGuideImageId
    ,lessonGuideImage.BlobId as LessonGuideImageBlobId
    ,lessonGuideImage.Name as LessonGuideImageName
    ,lessonGuideImage.Format as LessonGuideImageFormat
    ,lessonGuideImage.Width as LessonGuideImageWidth
    ,lessonGuideImage.Height as LessonGuideImageHeight
    ,lessonGuideImage.Caption as LessonGuideImageCaption
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
    left join [Framework].[ImageFile-Active] lessonGuideImage on lesson.GuideImageId = lessonGuideImage.Id
    inner join [Education].[Unit-Active] lessonUnit on lesson.UnitId = lessonUnit.Id
    left join [Framework].[ImageFile-Active] trophyImage on objective.TrophyImageId = trophyImage.Id
    inner join [Content].[Binder] binder on objective.BinderId = binder.Id
    inner join [Content].[BinderType-Active] binderType on binder.TypeId = binderType.Id
where objective.Id = @Id
    and objective.StatusId = @ContentStatusComplete