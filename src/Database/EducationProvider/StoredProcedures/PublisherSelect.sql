create proc [EducationProvider].[PublisherSelect] (
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
     publisher.Id
    ,publisher.OrganizationId
    ,(select count(1) from [Education].[PublisherContact-Active] where PublisherId = publisher.Id) as PublisherContactCount
from [Education].[Publisher-Active] publisher
    inner join [Framework].[Organization-Active] organization on publisher.OrganizationId = organization.Id
    inner join [Education].[Provider-Active] provider on publisher.ProviderId = provider.Id
where publisher.Id = @Id
    and provider.Id = @providerId