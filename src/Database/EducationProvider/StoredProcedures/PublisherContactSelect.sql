create proc [EducationProvider].[PublisherContactSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @providerId uniqueidentifier = (
    select top 1 provider.Id
    from [Education].[Provider-Active] provider
        inner join [Education].[ProviderContact-Active] providerContact on providerContact.ProviderId = provider.Id
        inner join [Framework].[User-Active] userTable on providerContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     publisherContact.Id
    ,publisherContact.PublisherId
    ,publisherContact.UserId
    ,publisherContact.ContactId
from [Education].[PublisherContact-Active] publisherContact
    inner join [Framework].[Contact-Active] contact on publisherContact.ContactId = contact.Id
    inner join [Education].[Publisher-Active] publisher on publisherContact.PublisherId = publisher.Id
    inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
    left join [Framework].[User-Active] userTable on publisherContact.UserId = userTable.Id
where publisherContact.Id = @Id
    and provider.Id = @providerId