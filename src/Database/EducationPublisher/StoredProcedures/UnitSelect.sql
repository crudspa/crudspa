create proc [EducationPublisher].[UnitSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     unit.Id
    ,unit.Title
    ,unit.StatusId
    ,status.Name as StatusName
    ,unit.GradeId
    ,grade.Name as GradeName
    ,unit.ParentId
    ,parent.Title as ParentTitle
    ,unit.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,unit.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,unit.GuideText
    ,guideAudio.Id as GuideAudioId
    ,guideAudio.BlobId as GuideAudioBlobId
    ,guideAudio.Name as GuideAudioName
    ,guideAudio.Format as GuideAudioFormat
    ,guideAudio.OptimizedStatus as GuideAudioOptimizedStatus
    ,guideAudio.OptimizedBlobId as GuideAudioOptimizedBlobId
    ,guideAudio.OptimizedFormat as GuideAudioOptimizedFormat
    ,introAudio.Id as IntroAudioId
    ,introAudio.BlobId as IntroAudioBlobId
    ,introAudio.Name as IntroAudioName
    ,introAudio.Format as IntroAudioFormat
    ,introAudio.OptimizedStatus as IntroAudioOptimizedStatus
    ,introAudio.OptimizedBlobId as IntroAudioOptimizedBlobId
    ,introAudio.OptimizedFormat as IntroAudioOptimizedFormat
    ,songAudio.Id as SongAudioId
    ,songAudio.BlobId as SongAudioBlobId
    ,songAudio.Name as SongAudioName
    ,songAudio.Format as SongAudioFormat
    ,songAudio.OptimizedStatus as SongAudioOptimizedStatus
    ,songAudio.OptimizedBlobId as SongAudioOptimizedBlobId
    ,songAudio.OptimizedFormat as SongAudioOptimizedFormat
    ,unit.Treatment
    ,unit.Control
    ,unit.Ordinal
    ,(select count(1) from [Education].[Lesson-Active] where UnitId = unit.Id) as LessonCount
    ,(select count(1) from [Education].[UnitBook-Active] where UnitId = unit.Id) as UnitBookCount
from [Education].[Unit-Active] unit
    left join [Education].[Achievement-Active] generatesAchievement on unit.GeneratesAchievementId = generatesAchievement.Id
    inner join [Education].[Grade-Active] grade on unit.GradeId = grade.Id
    left join [Framework].[AudioFile-Active] guideAudio on unit.GuideAudioId = guideAudio.Id
    inner join [Framework].[ImageFile-Active] image on unit.ImageId = image.Id
    left join [Framework].[AudioFile-Active] introAudio on unit.IntroAudioId = introAudio.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    left join [Education].[Unit-Active] parent on unit.ParentId = parent.Id
    left join [Education].[Achievement-Active] requiresAchievement on unit.RequiresAchievementId = requiresAchievement.Id
    left join [Framework].[AudioFile-Active] songAudio on unit.SongAudioId = songAudio.Id
    inner join [Framework].[ContentStatus-Active] status on unit.StatusId = status.Id
where unit.Id = @Id
    and organization.Id = @organizationId