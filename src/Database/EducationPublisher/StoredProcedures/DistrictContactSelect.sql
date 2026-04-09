create proc [EducationPublisher].[DistrictContactSelect] (
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
     districtContact.Id
    ,districtContact.DistrictId
    ,districtContact.Title
    ,districtContact.UserId
    ,districtContact.ContactId
    ,districtOrganization.Name as DistrictName
from [Education].[DistrictContact-Active] districtContact
    inner join [Framework].[Contact-Active] contact on districtContact.ContactId = contact.Id
    inner join [Education].[District-Active] district on districtContact.DistrictId = district.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    left join [Framework].[User-Active] userTable on districtContact.UserId = userTable.Id
    inner join [Framework].[Organization-Active] districtOrganization on district.OrganizationId = districtOrganization.Id
where districtContact.Id = @Id
    and publisher.Id = @publisherId