create trigger [Content].[ButtonTrigger] on [Content].[Button]
    for update
as

insert [Content].[Button] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,Internal
    ,Path
    ,Text
    ,TextAlignIndex
    ,LeftIconId
    ,RightIconId
    ,BoxId
    ,ShapeIndex
    ,GraphicIndex
    ,TextTypeIndex
    ,OrientationIndex
    ,IconId
    ,ImageId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.Internal
    ,deleted.Path
    ,deleted.Text
    ,deleted.TextAlignIndex
    ,deleted.LeftIconId
    ,deleted.RightIconId
    ,deleted.BoxId
    ,deleted.ShapeIndex
    ,deleted.GraphicIndex
    ,deleted.TextTypeIndex
    ,deleted.OrientationIndex
    ,deleted.IconId
    ,deleted.ImageId
from deleted