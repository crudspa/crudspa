create proc [EducationDistrict].[SchoolContactSelect] (
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
     schoolContact.Id
    ,schoolContact.SchoolId
    ,schoolContact.TitleId
    ,title.Name as TitleName
    ,schoolContact.TestAccount
    ,schoolContact.UserId
    ,schoolContact.ContactId
from [Education].[SchoolContact-Active] schoolContact
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Education].[Title-Active] title on schoolContact.TitleId = title.Id
    left join [Framework].[User-Active] userTable on schoolContact.UserId = userTable.Id
where schoolContact.Id = @Id
    and district.Id = @districtId