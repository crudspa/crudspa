create proc [EducationDistrict].[DistrictContactSelect] (
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
     districtContact.Id
    ,districtContact.Title
    ,districtContact.UserId
    ,districtContact.ContactId
from [Education].[DistrictContact-Active] districtContact
    inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
    inner join [Education].[District-Active] district on districtContact.DistrictId = district.Id
    left join [Framework].[User-Active] userTable on districtContact.UserId = userTable.Id
where districtContact.Id = @Id
    and district.Id = @districtId