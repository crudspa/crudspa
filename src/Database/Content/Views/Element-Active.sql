create view [Content].[Element-Active] as

select element.Id as Id
    ,element.SectionId as SectionId
    ,element.TypeId as TypeId
    ,element.RequireInteraction as RequireInteraction
    ,element.BoxId as BoxId
    ,element.ItemId as ItemId
    ,element.Ordinal as Ordinal
from [Content].[Element] element
where 1=1
    and element.IsDeleted = 0
    and element.VersionOf = element.Id