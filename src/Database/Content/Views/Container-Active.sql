create view [Content].[Container-Active] as

select container.Id as Id
    ,container.DirectionId as DirectionId
    ,container.WrapId as WrapId
    ,container.JustifyContentId as JustifyContentId
    ,container.AlignItemsId as AlignItemsId
    ,container.AlignContentId as AlignContentId
    ,container.Gap as Gap
from [Content].[Container] container
where 1=1
    and container.IsDeleted = 0
    and container.VersionOf = container.Id