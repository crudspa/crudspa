create proc [SamplesCatalog].[ColorSelectOrderables] (
     @SessionId uniqueidentifier
) as

set nocount on

select
     color.Id
    ,color.Name as Name
    ,color.Ordinal
from [Samples].[Color-Active] color
order by color.Ordinal