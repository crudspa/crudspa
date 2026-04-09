create proc [ContentDesign].[JustifyContentSelectOrderables] as

select
    justifyContent.Id
    ,justifyContent.Name
    ,justifyContent.Ordinal
from [Content].[JustifyContent-Active] justifyContent
order by justifyContent.Ordinal