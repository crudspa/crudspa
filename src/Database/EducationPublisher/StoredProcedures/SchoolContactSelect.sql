create proc [EducationPublisher].[SchoolContactSelect] (
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
     schoolContact.Id
    ,schoolContact.SchoolId
    ,schoolContact.TitleId
    ,title.Name as TitleName
    ,schoolContact.TestAccount
    ,schoolContact.Treatment
    ,schoolContact.ContactId
    ,schoolContact.UserId
    ,schoolOrganization.Name as SchoolName
    ,district.Id as DistrictId
    ,districtOrganization.Name as DistrictName
from [Education].[SchoolContact-Active] schoolContact
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    inner join [Education].[District-Active] district on school.DistrictId = district.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
    inner join [Education].[Title-Active] title on schoolContact.TitleId = title.Id
    left join [Framework].[User-Active] userTable on schoolContact.UserId = userTable.Id
    inner join [Framework].[Organization-Active] schoolOrganization on school.OrganizationId = schoolOrganization.Id
    inner join [Framework].[Organization-Active] districtOrganization on district.OrganizationId = districtOrganization.Id
where schoolContact.Id = @Id
    and publisher.Id = @publisherId