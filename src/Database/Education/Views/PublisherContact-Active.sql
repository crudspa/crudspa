create view [Education].[PublisherContact-Active] as

select publisherContact.Id as Id
    ,publisherContact.PublisherId as PublisherId
    ,publisherContact.ContactId as ContactId
    ,publisherContact.UserId as UserId
from [Education].[PublisherContact] publisherContact
where 1=1
    and publisherContact.IsDeleted = 0
    and publisherContact.VersionOf = publisherContact.Id