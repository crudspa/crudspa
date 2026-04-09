create proc [EducationStudent].[LessonSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @ConditionGroupControl uniqueidentifier = 'c02332b0-14ee-4897-aa56-4aeba64e8bad'
declare @ConditionGroupTreatment uniqueidentifier = '3befe15f-f053-4f26-826f-583dead12346'

declare @studentId uniqueidentifier
declare @studentConditionGroupId uniqueidentifier

select top 1
     @studentId = student.Id
    ,@studentConditionGroupId = student.ConditionGroupId
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
        and session.Id = @SessionId

insert [Education].[LessonViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,LessonId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

create table #LicensedLessons (
    Id uniqueidentifier not null
)

insert #LicensedLessons (Id)
select lesson.Id
from [Education].[Lesson-Active] lesson
where lesson.Id = @Id
    and exists (
        select 1
        from [EducationStudent].[UnitLicenses](@SessionId, lesson.UnitId) unitLicense
        where unitLicense.AllLessons = 1
            or exists (
                select 1
                from [Education].[UnitLicenseLesson-Active] unitLicenseLesson
                where unitLicenseLesson.UnitLicenseId = unitLicense.Id
                    and unitLicenseLesson.LessonId = lesson.Id
            )
    )

select
     lesson.Id as Id
    ,lesson.Title as Title
    ,lesson.StatusId as StatusId
    ,lesson.UnitId as UnitId
    ,lesson.ImageId as ImageId
    ,lesson.GuideImageId as GuideImageId
    ,lesson.GuideText as GuideText
    ,lesson.GuideAudioId as GuideAudioId
    ,lesson.RequireSequentialCompletion as RequireSequentialCompletion
    ,lesson.Ordinal as Ordinal
    ,guideAudio.Id as GuideAudioId
    ,guideAudio.BlobId as GuideAudioBlobId
    ,guideAudio.Name as GuideAudioName
    ,guideAudio.Format as GuideAudioFormat
    ,guideAudio.OptimizedStatus as GuideAudioOptimizedStatus
    ,guideAudio.OptimizedBlobId as GuideAudioOptimizedBlobId
    ,guideAudio.OptimizedFormat as GuideAudioOptimizedFormat
    ,guideImage.Id as GuideImageId
    ,guideImage.BlobId as GuideImageBlobId
    ,guideImage.Name as GuideImageName
    ,guideImage.Format as GuideImageFormat
    ,guideImage.Width as GuideImageWidth
    ,guideImage.Height as GuideImageHeight
    ,guideImage.Caption as GuideImageCaption
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,unit.Title as UnitTitle
from [Education].[Lesson-Active] lesson
    left join [Framework].[AudioFile-Active] guideAudio on lesson.GuideAudioId = guideAudio.Id
    left join [Framework].[ImageFile-Active] guideImage on lesson.GuideImageId = guideImage.Id
    inner join [Framework].[ImageFile-Active] image on lesson.ImageId = image.Id
    inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
    inner join #LicensedLessons licensedLesson on licensedLesson.Id = lesson.Id
where lesson.Id = @Id
    and lesson.StatusId = @ContentStatusComplete
    and (
        (lesson.Treatment = 1 and lesson.Control = 1)
        or (lesson.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (lesson.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and (lesson.RequiresAchievementId is null
        or exists (
            select 1
            from [Education].[StudentAchievement-Active] studentAchievement
            where studentAchievement.StudentId = @studentId
                and studentAchievement.AchievementId = lesson.RequiresAchievementId
        )
    )

select
     objective.Id
    ,objective.Title
    ,objective.LessonId
    ,objective.BinderId
    ,objective.Ordinal
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
from [Education].[Objective-Active] objective
    left join [Framework].[ImageFile-Active] trophyImage on objective.TrophyImageId = trophyImage.Id
    inner join [Education].[Lesson-Active] lesson on objective.LessonId = lesson.Id
    inner join #LicensedLessons licensedLesson on licensedLesson.Id = lesson.Id
where objective.LessonId = @Id
    and objective.StatusId = @ContentStatusComplete
    and lesson.StatusId = @ContentStatusComplete
    and (
        (lesson.Treatment = 1 and lesson.Control = 1)
        or (lesson.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (lesson.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and (lesson.RequiresAchievementId is null
        or exists (
            select 1
            from [Education].[StudentAchievement-Active] lessonStudentAchievement
            where lessonStudentAchievement.StudentId = @studentId
                and lessonStudentAchievement.AchievementId = lesson.RequiresAchievementId
        )
    )
    and (objective.RequiresAchievementId is null
        or exists (
            select 1
            from [Education].[StudentAchievement-Active] objectiveStudentAchievement
            where objectiveStudentAchievement.StudentId = @studentId
                and objectiveStudentAchievement.AchievementId = objective.RequiresAchievementId
        )
    )
order by objective.Ordinal