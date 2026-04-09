create proc [EducationStudent].[LessonAllObjectivesAreCompleted] (
     @SessionId uniqueidentifier
    ,@LessonId uniqueidentifier
    ,@ObjectiveId uniqueidentifier
    ,@AllAreComplete bit output
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)
if (@LessonId is null and @ObjectiveId is not null)
begin
    set @LessonId = (select top 1 LessonId from [Education].[Objective-Active] where Id = @ObjectiveId)
end

if (exists (
    select lesson.Id
    from [Education].[Lesson-Active] lesson
        inner join [Education].[Objective-Active] objective on objective.LessonId = lesson.Id
    where lesson.Id = @LessonId
        and objective.Id not in (select ObjectiveId from [Education].[ObjectiveCompleted] where StudentId = @studentId)
        and objective.StatusId = @ContentStatusComplete
        and (objective.RequiresAchievementId is null
            or exists (
                select Id
                from [Education].[StudentAchievement-Active]
                where StudentId = @studentId
                    and AchievementId = objective.RequiresAchievementId
            )
        )
    )
)
begin
    set @AllAreComplete = 0
end
else
begin
    set @AllAreComplete = 1
end