create proc [EducationDistrict].[SchoolSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @districtId uniqueidentifier = (
    select top 1 district.Id
    from [Education].[District-Active] district
        inner join [Education].[DistrictContact-Active] districtContact on districtContact.DistrictId = district.Id
        inner join [Framework].[User-Active] userTable on districtContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     school.Id
    ,school.[Key]
    ,school.CommunityId
    ,community.Name as CommunityName
    ,school.Treatment
    ,school.AddressId
    ,school.OrganizationId
    ,(select count(1) from [Education].[Classroom-Active] where SchoolId = school.Id) as ClassroomCount
    ,(select count(1) from [Education].[SchoolContact-Active] where SchoolId = school.Id) as SchoolContactCount
from [Education].[School-Active] school
    left join [Framework].[UsaPostal-Active] address on school.AddressId = address.Id
    left join [Education].[Community-Active] community on school.CommunityId = community.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
where school.Id = @Id
    and district.Id = @districtId