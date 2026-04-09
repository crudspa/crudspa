create proc [EducationPublisher].[UnitLicenseSelectForLicense] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     unitLicense.Id
    ,unitLicense.LicenseId
    ,unitLicense.UnitId
    ,unit.Title as UnitTitle
    ,unitLicense.AllBooks
    ,unitLicense.AllLessons
from [Education].[UnitLicense-Active] unitLicense
    inner join [Framework].[License-Active] license on unitLicense.LicenseId = license.Id
    inner join [Education].[Unit-Active] unit on unitLicense.UnitId = unit.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
where unitLicense.LicenseId = @LicenseId
    and organization.Id = @organizationId

select distinct
     unitLicense.Id as RootId
    ,book.Id as Id
    ,book.Title as Name
    ,convert(bit, case when unitLicense.AllBooks = 1 or unitLicenseBook.Id is not null then 1 else 0 end) as Selected
from [Education].[UnitLicense-Active] unitLicense
    inner join [Education].[Unit-Active] unit on unit.Id = unitLicense.UnitId
    inner join [Education].[UnitBook-Active] unitBook on unitBook.UnitId = unit.Id
    inner join [Education].[Book-Active] book on book.Id = unitBook.BookId
    left join [Education].[UnitLicenseBook-Active] unitLicenseBook on unitLicenseBook.UnitLicenseId = unitLicense.Id
        and unitLicenseBook.BookId = book.Id
where unitLicense.LicenseId = @LicenseId
order by book.Title

select distinct
     unitLicense.Id as RootId
    ,lesson.Id as Id
    ,lesson.Title as Name
    ,convert(bit, case when unitLicense.AllLessons = 1 or unitLicenseLesson.Id is not null then 1 else 0 end) as Selected
    ,lesson.Ordinal
from [Education].[UnitLicense-Active] unitLicense
    inner join [Education].[Unit-Active] unit on unit.Id = unitLicense.UnitId
    inner join [Education].[Lesson-Active] lesson on lesson.UnitId = unit.Id
    left join [Education].[UnitLicenseLesson-Active] unitLicenseLesson on unitLicenseLesson.UnitLicenseId = unitLicense.Id
        and unitLicenseLesson.LessonId = lesson.Id
where unitLicense.LicenseId = @LicenseId
order by lesson.Ordinal