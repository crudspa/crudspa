create proc [EducationProvider].[ProviderContactSelect] (
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

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     providerContact.Id
    ,providerContact.UserId
    ,providerContact.ContactId
from [Education].[ProviderContact-Active] providerContact
    inner join [Framework].[Contact-Active] contact on providerContact.ContactId = contact.Id
    inner join [Education].[Provider-Active] provider on providerContact.ProviderId = provider.Id
    left join [Framework].[User-Active] userTable on providerContact.UserId = userTable.Id
where providerContact.Id = @Id
    and provider.Id = @providerId
    and provider.OrganizationId = @organizationId