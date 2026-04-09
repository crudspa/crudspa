create trigger [Content].[ContainerTrigger] on [Content].[Container]
    for update
as

insert [Content].[Container] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,DirectionId
    ,WrapId
    ,JustifyContentId
    ,AlignItemsId
    ,AlignContentId
    ,Gap
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.DirectionId
    ,deleted.WrapId
    ,deleted.JustifyContentId
    ,deleted.AlignItemsId
    ,deleted.AlignContentId
    ,deleted.Gap
from deleted