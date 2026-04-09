create proc [ContentDesign].[AlignContentSelectOrderables] as

select
    alignContent.Id
    ,alignContent.Name
    ,alignContent.Ordinal
from [Content].[AlignContent-Active] alignContent
order by alignContent.Ordinal