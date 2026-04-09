create view [Samples].[CatalogContact-Active] as

select catalogContact.Id as Id
    ,catalogContact.ContactId as ContactId
    ,catalogContact.UserId as UserId
    ,catalogContact.LastIp as LastIp
from [Samples].[CatalogContact] catalogContact
where 1=1
    and catalogContact.IsDeleted = 0
    and catalogContact.VersionOf = catalogContact.Id