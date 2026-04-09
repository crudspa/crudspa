create proc [SamplesCatalog].[CatalogSelectRoleNames] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     role.Id
    ,role.Name
from [Framework].[Role-Active] role
    inner join [Framework].[Organization-Active] organization on role.OrganizationId = organization.Id
    inner join [Samples].[Catalog-Active] catalog on catalog.OrganizationId = organization.Id

where 1 = 1

order by role.Name