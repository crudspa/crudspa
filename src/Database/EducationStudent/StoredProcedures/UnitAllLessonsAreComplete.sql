create proc [EducationStudent].[UnitAllLessonsAreComplete] (
     @SessionId uniqueidentifier
    ,@UnitId uniqueidentifier
    ,@AllAreComplete bit output
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @ConditionGroupControl uniqueidentifier = 'c02332b0-14ee-4897-aa56-4aeba64e8bad'
declare @ConditionGroupTreatment uniqueidentifier = '3befe15f-f053-4f26-826f-583dead12346'

declare @studentId uniqueidentifier
declare @studentConditionGroupId uniqueidentifier

select
     @studentId = student.Id
    ,@studentConditionGroupId = student.ConditionGroupId
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = userTable.Id and session.Id = @SessionId

create table #LicensedLessons (
    Id uniqueidentifier not null
)

insert #LicensedLessons (Id)
select distinct lesson.Id
from [Education].[Lesson-Active] lesson
where lesson.UnitId = @UnitId
    and exists (
        select 1
        from [EducationStudent].[UnitLicenses](@SessionId, @UnitId) unitLicense
        where unitLicense.AllLessons = 1
            or exists (
                select 1
                from [Education].[UnitLicenseLesson-Active] unitLicenseLesson
                where unitLicenseLesson.UnitLicenseId = unitLicense.Id
                    and unitLicenseLesson.LessonId = lesson.Id
            )
    )

if (exists(
    select 1
    from [Education].[Lesson-Active] lesson
        inner join #LicensedLessons licensedLesson on licensedLesson.Id = lesson.Id
        inner join [Education].[Objective-Active] objective on objective.LessonId = lesson.Id
    where lesson.UnitId = @UnitId
        and lesson.StatusId = @ContentStatusComplete
        and objective.StatusId = @ContentStatusComplete
        and not exists (
            select 1
            from [Education].[ObjectiveCompleted] completed
            where completed.StudentId = @studentId
                and completed.ObjectiveId = objective.Id
        )
        and (objective.RequiresAchievementId is null
            or exists (
                select 1
                from [Education].[StudentAchievement-Active] objectiveStudentAchievement
                where objectiveStudentAchievement.StudentId = @studentId
                    and objectiveStudentAchievement.AchievementId = objective.RequiresAchievementId
            )
        )
        and (lesson.RequiresAchievementId is null
            or exists (
                select 1
                from [Education].[StudentAchievement-Active] lessonStudentAchievement
                where lessonStudentAchievement.StudentId = @studentId
                    and lessonStudentAchievement.AchievementId = lesson.RequiresAchievementId
            )
        )
        and (
            (lesson.Treatment = 1 and lesson.Control = 1)
            or (lesson.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
            or (lesson.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
        )
))
begin
    set @AllAreComplete = 0
end
else
begin
    set @AllAreComplete = 1
end