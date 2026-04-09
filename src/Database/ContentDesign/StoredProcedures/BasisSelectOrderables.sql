create proc [ContentDesign].[BasisSelectOrderables] as

select
    basis.Id
    ,basis.Name
    ,basis.Ordinal
from [Content].[Basis-Active] basis
order by basis.Ordinal