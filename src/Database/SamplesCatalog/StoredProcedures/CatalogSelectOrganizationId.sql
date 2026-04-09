create proc [SamplesCatalog].[CatalogSelectOrganizationId] as

set nocount on

select top 1
    catalog.OrganizationId
from [Samples].[Catalog-Active] catalog
    inner join [Framework].[Organization-Active] organization on catalog.OrganizationId = organization.Id
order by catalog.Id