create trigger [Education].[UnitLicenseLessonTrigger] on [Education].[UnitLicenseLesson]
    for update
as

insert [Education].[UnitLicenseLesson] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,UnitLicenseId
    ,LessonId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.UnitLicenseId
    ,deleted.LessonId
from deleted