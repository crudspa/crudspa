create view [Education].[UnitLicenseLesson-Active] as

select unitLicenseLesson.Id as Id
    ,unitLicenseLesson.UnitLicenseId as UnitLicenseId
    ,unitLicenseLesson.LessonId as LessonId
from [Education].[UnitLicenseLesson] unitLicenseLesson
where 1=1
    and unitLicenseLesson.IsDeleted = 0
    and unitLicenseLesson.VersionOf = unitLicenseLesson.Id