create function [EducationStudent].[LicensedLessons] (
    @SessionId uniqueidentifier,
    @LessonId uniqueidentifier = null
)
returns table
as
return
(
    select distinct
        lesson.Id as LessonId,
        lesson.UnitId
    from [Education].[Lesson-Active] lesson
        cross apply [EducationStudent].[UnitLicenses](@SessionId, lesson.UnitId) unitLicense
    where (@LessonId is null or lesson.Id = @LessonId)
        and (
            unitLicense.AllLessons = 1
            or exists (
                select 1
                from [Education].[UnitLicenseLesson-Active] unitLicenseLesson
                where unitLicenseLesson.UnitLicenseId = unitLicense.Id
                    and unitLicenseLesson.LessonId = lesson.Id
            )
        )
);