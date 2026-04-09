create proc [ContentDesign].[AlignItemsSelectOrderables] as

select
    alignItems.Id
    ,alignItems.Name
    ,alignItems.Ordinal
from [Content].[AlignItems-Active] alignItems
order by alignItems.Ordinal