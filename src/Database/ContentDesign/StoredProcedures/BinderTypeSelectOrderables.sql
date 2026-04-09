create proc [ContentDesign].[BinderTypeSelectOrderables] as

set nocount on
select
    binderType.Id
    ,binderType.Name
    ,binderType.Ordinal
from [Content].[BinderType-Active] binderType
order by binderType.Ordinal