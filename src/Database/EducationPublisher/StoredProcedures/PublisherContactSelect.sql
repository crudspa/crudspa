create proc [EducationPublisher].[PublisherContactSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

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
     publisherContact.Id
    ,publisherContact.UserId
    ,publisherContact.ContactId
from [Education].[PublisherContact-Active] publisherContact
    inner join [Framework].[Contact-Active] contact on publisherContact.ContactId = contact.Id
    inner join [Education].[Publisher-Active] publisher on publisherContact.PublisherId = publisher.Id
    left join [Framework].[User-Active] userTable on publisherContact.UserId = userTable.Id
where publisherContact.Id = @Id
    and publisher.OrganizationId = @organizationId
    and publisher.Id = @publisherId