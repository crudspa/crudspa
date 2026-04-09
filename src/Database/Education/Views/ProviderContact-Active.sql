create view [Education].[ProviderContact-Active] as

select providerContact.Id as Id
    ,providerContact.ProviderId as ProviderId
    ,providerContact.ContactId as ContactId
    ,providerContact.UserId as UserId
from [Education].[ProviderContact] providerContact
where 1=1
    and providerContact.IsDeleted = 0
    and providerContact.VersionOf = providerContact.Id