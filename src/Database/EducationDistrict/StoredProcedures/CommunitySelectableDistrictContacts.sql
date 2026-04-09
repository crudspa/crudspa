create proc [EducationDistrict].[CommunitySelectableDistrictContacts] (
     @SessionId uniqueidentifier
    ,@CommunityId uniqueidentifier
) as

select
     @CommunityId as RootId
    ,districtContact.Id as Id
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
    ,convert(bit, case when communitySteward.Id is null then 0 else 1 end) as Selected
from [Education].[DistrictContact-Active] districtContact
    inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
    left join [Education].[CommunitySteward-Active] communitySteward on communitySteward.DistrictContactId = districtContact.Id
        and communitySteward.CommunityId = @CommunityId
order by trim(contact.FirstName + ' ' + contact.LastName)