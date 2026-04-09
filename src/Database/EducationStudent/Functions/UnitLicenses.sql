create function [EducationStudent].[UnitLicenses] (
     @SessionId uniqueidentifier
    ,@UnitId uniqueidentifier = null
)
returns table
as

return

with SessionStudent as (
    select top 1
         student.Id as StudentId
        ,school.Id as SchoolId
        ,district.Id as DistrictId
    from [Education].[Student-Active] student
        inner join [Education].[Family-Active] family on student.FamilyId = family.Id
        inner join [Education].[School-Active] school on family.SchoolId = school.Id
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] users on users.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = users.Id
    where session.Id = @SessionId
)
select distinct
     unitLicense.Id as Id
    ,unitLicense.UnitId as UnitId
    ,unitLicense.LicenseId as LicenseId
    ,unitLicense.AllBooks as AllBooks
    ,unitLicense.AllLessons as AllLessons
    ,sessionStudent.StudentId as StudentId
    ,sessionStudent.SchoolId as SchoolId
    ,sessionStudent.DistrictId as DistrictId
from SessionStudent sessionStudent
    cross join [Education].[UnitLicense-Active] unitLicense
    inner join [Education].[DistrictLicense-Active] districtLicense on districtLicense.LicenseId = unitLicense.LicenseId
        and districtLicense.DistrictId = sessionStudent.DistrictId
    left join [Education].[DistrictLicenseSchool-Active] districtLicenseSchool on districtLicenseSchool.DistrictLicenseId = districtLicense.Id
where (@UnitId is null or unitLicense.UnitId = @UnitId)
    and (districtLicense.AllSchools = 1 or districtLicenseSchool.SchoolId = sessionStudent.SchoolId)