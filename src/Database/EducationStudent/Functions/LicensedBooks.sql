create function [EducationStudent].[LicensedBooks] (
    @SessionId uniqueidentifier,
    @BookId uniqueidentifier = null
)
returns table
as
return
(
    select distinct
        unitBook.BookId,
        unitBook.UnitId
    from [Education].[UnitBook-Active] unitBook
        cross apply [EducationStudent].[UnitLicenses](@SessionId, unitBook.UnitId) unitLicense
    where (@BookId is null or unitBook.BookId = @BookId)
        and (
            unitLicense.AllBooks = 1
            or exists (
                select 1
                from [Education].[UnitLicenseBook-Active] unitLicenseBook
                where unitLicenseBook.UnitLicenseId = unitLicense.Id
                    and unitLicenseBook.BookId = unitBook.BookId
            )
        )
);