create proc [SamplesCatalog].[BrandSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     brand.Id
    ,brand.Name as Name
    ,brand.Ordinal
from [Samples].[Brand-Active] brand
order by brand.Ordinal