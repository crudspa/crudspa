create proc [EducationStudent].[ObjectiveSelectForLesson] (
     @LessonId uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

declare @studentId uniqueidentifier

select top 1
    @studentId = student.Id
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] users on users.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = users.Id
        and session.Id = @SessionId

select
    objective.Id as Id
    ,objective.Title as Title
    ,objective.BinderId as BinderId
    ,objective.Ordinal as Ordinal
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
from [Education].[Objective-Active] objective
    left join [Framework].[ImageFile-Active] trophyImage on objective.TrophyImageId = trophyImage.Id
where objective.LessonId = @LessonId
    and objective.StatusId = @ContentStatusComplete
    and (objective.RequiresAchievementId is null
        or exists (
            select Id
            from [Education].[StudentAchievement-Active]
            where StudentId = @studentId
                and AchievementId = objective.RequiresAchievementId
        )
    )
order by objective.Ordinal