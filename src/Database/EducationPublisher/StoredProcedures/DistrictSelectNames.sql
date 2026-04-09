create proc [EducationPublisher].[DistrictSelectNames] (
     @SessionId uniqueidentifier
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
     district.Id
    ,organization.Name
from [Education].[District-Active] district
    inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
    inner join [Education].[Publisher-Active] publisher on district.PublisherId = publisher.Id
where publisher.Id = @publisherId
order by organization.Name