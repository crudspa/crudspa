create proc [SamplesCatalog].[CatalogSelect] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     catalog.Id
    ,catalog.OrganizationId
from [Samples].[Catalog-Active] catalog
    inner join [Framework].[Organization-Active] organization on catalog.OrganizationId = organization.Id
where 1 = 1
    and catalog.OrganizationId = (
        select top 1 userTable.OrganizationId
        from [Framework].[Session-Active] session
            inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
        where session.Id = @SessionId
            and session.Ended is null
    )