create proc [EducationPublisher].[CommunitySelectableDistrictContacts] (
     @SessionId uniqueidentifier
    ,@CommunityId uniqueidentifier
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @districtId uniqueidentifier = (
    select top 1 community.DistrictId
    from [Education].[Community-Active] community
        inner join [Education].[District-Active] district on community.DistrictId = district.Id
    where community.Id = @CommunityId
        and district.PublisherId = @publisherId
)

select
     @CommunityId as RootId
    ,districtContact.Id as Id
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
    ,convert(bit, case when communitySteward.Id is null then 0 else 1 end) as Selected
from [Education].[DistrictContact-Active] districtContact
    inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
    left join [Education].[CommunitySteward-Active] communitySteward on communitySteward.DistrictContactId = districtContact.Id
        and communitySteward.CommunityId = @CommunityId
where districtContact.DistrictId = @districtId
order by trim(contact.FirstName + ' ' + contact.LastName)