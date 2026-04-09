create proc [ContentDesign].[ElementTypeSelectFull] as

set nocount on
select
    elementType.Id
    ,elementType.Name
    ,elementType.IconId
    ,elementType.EditorView
    ,elementType.DisplayView
    ,elementType.RepositoryClass
    ,elementType.OnlyChild
    ,elementType.SupportsInteraction
    ,elementType.Ordinal
    ,icon.CssClass as IconCssClass
from [Content].[ElementType-Active] elementType
    left join [Framework].[Icon-Active] icon on elementType.IconId = icon.Id

order by elementType.Ordinal