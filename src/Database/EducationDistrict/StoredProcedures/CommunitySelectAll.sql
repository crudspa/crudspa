create proc [EducationDistrict].[CommunitySelectAll] (
     @SessionId uniqueidentifier
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
     community.Id
    ,community.Name
    ,(select count(1) from [Education].[School-Active] where CommunityId = community.Id) as SchoolCount
from [Education].[Community-Active] community
    inner join [Education].[District-Active] district on community.DistrictId = district.Id
where 1 = 1
    and district.Id = @districtId

select
    communitySteward.Id
    ,communitySteward.CommunityId
    ,communitySteward.DistrictContactId
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
from [Education].[CommunitySteward-Active] communitySteward
    inner join [Education].[Community-Active] community on communitySteward.CommunityId = community.Id
    inner join [Education].[DistrictContact-Active] districtContact on communitySteward.DistrictContactId = districtContact.Id
    inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
where community.DistrictId = @districtId