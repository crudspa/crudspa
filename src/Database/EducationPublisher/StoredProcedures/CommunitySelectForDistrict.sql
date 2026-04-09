create proc [EducationPublisher].[CommunitySelectForDistrict] (
     @SessionId uniqueidentifier
    ,@DistrictId uniqueidentifier
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select
    community.Id
    ,community.DistrictId
    ,community.Name
    ,(select count(1) from [Education].[School-Active] where CommunityId = community.Id) as SchoolCount
from [Education].[Community-Active] community
    inner join [Education].[District-Active] district on community.DistrictId = district.Id
where community.DistrictId = @DistrictId
    and district.PublisherId = @publisherId

select
    communitySteward.Id
    ,communitySteward.CommunityId
    ,communitySteward.DistrictContactId
    ,trim(contact.FirstName + ' ' + contact.LastName) as Name
from [Education].[CommunitySteward-Active] communitySteward
    inner join [Education].[Community-Active] community on communitySteward.CommunityId = community.Id
    inner join [Education].[District-Active] district on community.DistrictId = district.Id
    inner join [Education].[DistrictContact-Active] districtContact on communitySteward.DistrictContactId = districtContact.Id
    inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
where community.DistrictId = @DistrictId
    and district.PublisherId = @publisherId