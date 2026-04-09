create proc [ContentDesign].[PageTypeSelectOrderables] as

set nocount on
select
    pageType.Id
    ,pageType.Name
    ,pageType.Ordinal
from [Content].[PageType-Active] pageType
order by pageType.Ordinal