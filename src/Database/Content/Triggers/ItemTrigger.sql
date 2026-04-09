create trigger [Content].[ItemTrigger] on [Content].[Item]
    for update
as

insert [Content].[Item] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BasisId
    ,BasisAmount
    ,Grow
    ,Shrink
    ,AlignSelfId
    ,MaxWidth
    ,MinWidth
    ,Width
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BasisId
    ,deleted.BasisAmount
    ,deleted.Grow
    ,deleted.Shrink
    ,deleted.AlignSelfId
    ,deleted.MaxWidth
    ,deleted.MinWidth
    ,deleted.Width
from deleted