create view [Content].[ElementType-Active] as

select elementType.Id as Id
    ,elementType.Name as Name
    ,elementType.IconId as IconId
    ,elementType.EditorView as EditorView
    ,elementType.DisplayView as DisplayView
    ,elementType.RepositoryClass as RepositoryClass
    ,elementType.OnlyChild as OnlyChild
    ,elementType.SupportsInteraction as SupportsInteraction
    ,elementType.Ordinal as Ordinal
from [Content].[ElementType] elementType
where 1=1
    and elementType.IsDeleted = 0