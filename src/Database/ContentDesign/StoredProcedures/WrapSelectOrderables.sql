create proc [ContentDesign].[WrapSelectOrderables] as

select
    wrap.Id
    ,wrap.Name
    ,wrap.Ordinal
from [Content].[Wrap-Active] wrap
order by wrap.Ordinal