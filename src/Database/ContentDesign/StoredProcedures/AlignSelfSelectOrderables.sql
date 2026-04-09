create proc [ContentDesign].[AlignSelfSelectOrderables] as

select
    alignSelf.Id
    ,alignSelf.Name
    ,alignSelf.Ordinal
from [Content].[AlignSelf-Active] alignSelf
order by alignSelf.Ordinal