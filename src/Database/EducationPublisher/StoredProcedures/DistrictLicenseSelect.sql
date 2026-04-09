create proc [EducationPublisher].[DistrictLicenseSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     districtLicense.Id
    ,districtLicense.LicenseId
    ,districtLicense.DistrictId
    ,organization.Name as DistrictName
    ,districtLicense.AllSchools
from [Education].[DistrictLicense-Active] districtLicense
    inner join [Education].[District-Active] district on districtLicense.DistrictId = district.Id
    inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
    inner join [Framework].[License-Active] license on districtLicense.LicenseId = license.Id
where districtLicense.Id = @Id
    and district.PublisherId = @publisherId


select distinct
     @Id as RootId
    ,school.Id as Id
    ,organization.Name as SchoolName
    ,convert(bit, case when districtLicense.AllSchools = 1 or districtLicenseSchool.Id is not null then 1 else 0 end) as Selected
from [Education].[DistrictLicense-Active] districtLicense
    inner join [Education].[District-Active] district on district.Id = districtLicense.DistrictId
    inner join [Education].[School-Active] school on school.DistrictId = district.Id
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
    left join [Education].[DistrictLicenseSchool-Active] districtLicenseSchool on districtLicenseSchool.DistrictLicenseId = districtLicense.Id
        and districtLicenseSchool.SchoolId = school.Id
where districtLicense.Id = @Id
    and district.PublisherId = @publisherId
order by organization.Name