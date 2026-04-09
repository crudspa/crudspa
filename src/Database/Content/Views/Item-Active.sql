create view [Content].[Item-Active] as

select item.Id as Id
    ,item.BasisId as BasisId
    ,item.BasisAmount as BasisAmount
    ,item.Grow as Grow
    ,item.Shrink as Shrink
    ,item.AlignSelfId as AlignSelfId
    ,item.MaxWidth as MaxWidth
    ,item.MinWidth as MinWidth
    ,item.Width as Width
from [Content].[Item] item
where 1=1
    and item.IsDeleted = 0
    and item.VersionOf = item.Id