create trigger [Content].[FontFaceTrigger] on [Content].[FontFace]
    for update
as

insert [Content].[FontFace] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,FontId
    ,FileId
    ,Style
    ,WeightMin
    ,WeightMax
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.FontId
    ,deleted.FileId
    ,deleted.Style
    ,deleted.WeightMin
    ,deleted.WeightMax
from deleted