create view [Samples].[Catalog-Active] as

select catalog.Id as Id
    ,catalog.OrganizationId as OrganizationId
from [Samples].[Catalog] catalog
where 1=1
    and catalog.IsDeleted = 0
    and catalog.VersionOf = catalog.Id