create proc [ContentDesign].[DirectionSelectOrderables] as

select
    direction.Id
    ,direction.Name
    ,direction.Ordinal
from [Content].[Direction-Active] direction
order by direction.Ordinal