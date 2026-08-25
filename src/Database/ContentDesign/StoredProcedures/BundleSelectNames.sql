create proc [ContentDesign].[BundleSelectNames]
as

set nocount on

select
     bundle.Id
    ,bundle.Name
from [Content].[Bundle-Active] bundle
order by bundle.Name