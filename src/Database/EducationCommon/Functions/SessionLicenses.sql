create function [EducationCommon].[SessionLicenses] (
    @SessionId uniqueidentifier
)
returns table
as
return
(
    with SessionContext as (
        select
            session.UserId,
            userTable.ContactId
        from [Framework].[Session-Active] session
            inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
        where session.Id = @SessionId
            and session.Ended is null
    ),
    DistrictContactDistricts as (
        select districtContact.DistrictId
        from SessionContext sessionContext
            inner join [Education].[DistrictContact-Active] districtContact on districtContact.UserId = sessionContext.UserId
        where districtContact.DistrictId is not null

        union

        select districtContact.DistrictId
        from SessionContext sessionContext
            inner join [Education].[DistrictContact-Active] districtContact on districtContact.ContactId = sessionContext.ContactId
        where districtContact.DistrictId is not null
    ),
    SessionSchools as (
        select schoolContact.SchoolId
        from SessionContext sessionContext
            inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.UserId = sessionContext.UserId
        where schoolContact.SchoolId is not null

        union

        select schoolContact.SchoolId
        from SessionContext sessionContext
            inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.ContactId = sessionContext.ContactId
        where schoolContact.SchoolId is not null

        union

        select family.SchoolId
        from SessionContext sessionContext
            inner join [Education].[Student-Active] student on student.ContactId = sessionContext.ContactId
            inner join [Education].[Family-Active] family on family.Id = student.FamilyId
        where family.SchoolId is not null
    ),
    SessionDistricts as (
        select districtContactDistrict.DistrictId
        from DistrictContactDistricts districtContactDistrict

        union

        select school.DistrictId
        from SessionSchools sessionSchool
            inner join [Education].[School-Active] school on school.Id = sessionSchool.SchoolId
        where school.DistrictId is not null
    )
    select distinct
        districtLicense.LicenseId
    from [Education].[DistrictLicense-Active] districtLicense
        inner join [Framework].[License-Active] license on license.Id = districtLicense.LicenseId
        inner join SessionDistricts sessionDistrict on sessionDistrict.DistrictId = districtLicense.DistrictId
    where districtLicense.LicenseId is not null
        and (
            districtLicense.AllSchools = 1
            or exists (
                select 1
                from DistrictContactDistricts districtContactDistrict
                where districtContactDistrict.DistrictId = districtLicense.DistrictId
            )
            or exists (
                select 1
                from [Education].[DistrictLicenseSchool-Active] districtLicenseSchool
                    inner join SessionSchools sessionSchool on sessionSchool.SchoolId = districtLicenseSchool.SchoolId
                where districtLicenseSchool.DistrictLicenseId = districtLicense.Id
            )
        )
);