create view [Education].[DistrictContact-Active] as

select districtContact.Id as Id
    ,districtContact.DistrictId as DistrictId
    ,districtContact.ContactId as ContactId
    ,districtContact.UserId as UserId
    ,districtContact.Title as Title
from [Education].[DistrictContact] districtContact
where 1=1
    and districtContact.IsDeleted = 0
    and districtContact.VersionOf = districtContact.Id