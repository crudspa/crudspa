create proc [EducationPublisher].[LessonSelectForUnit] (
     @SessionId uniqueidentifier
    ,@UnitId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     lesson.Id
    ,lesson.UnitId
    ,unit.Title as UnitTitle
    ,lesson.Title
    ,lesson.StatusId
    ,status.Name as StatusName
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
    ,lesson.GuideText
    ,guideAudio.Id as GuideAudioId
    ,guideAudio.BlobId as GuideAudioBlobId
    ,guideAudio.Name as GuideAudioName
    ,guideAudio.Format as GuideAudioFormat
    ,guideAudio.OptimizedStatus as GuideAudioOptimizedStatus
    ,guideAudio.OptimizedBlobId as GuideAudioOptimizedBlobId
    ,guideAudio.OptimizedFormat as GuideAudioOptimizedFormat
    ,lesson.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,lesson.RequireSequentialCompletion
    ,lesson.Treatment
    ,lesson.Control
    ,lesson.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,lesson.Ordinal
    ,(select count(1) from [Education].[Objective-Active] where LessonId = lesson.Id) as ObjectiveCount
from [Education].[Lesson-Active] lesson
    left join [Education].[Achievement-Active] generatesAchievement on lesson.GeneratesAchievementId = generatesAchievement.Id
    left join [Framework].[AudioFile-Active] guideAudio on lesson.GuideAudioId = guideAudio.Id
    left join [Framework].[ImageFile-Active] guideImage on lesson.GuideImageId = guideImage.Id
    inner join [Framework].[ImageFile-Active] image on lesson.ImageId = image.Id
    left join [Education].[Achievement-Active] requiresAchievement on lesson.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on lesson.StatusId = status.Id
    inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
where lesson.UnitId = @UnitId
    and organization.Id = @organizationId