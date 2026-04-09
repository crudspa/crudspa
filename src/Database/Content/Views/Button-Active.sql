create view [Content].[Button-Active] as

select button.Id as Id
    ,button.Internal as Internal
    ,button.Path as Path
    ,button.Text as Text
    ,button.TextAlignIndex as TextAlignIndex
    ,button.LeftIconId as LeftIconId
    ,button.RightIconId as RightIconId
    ,button.BoxId as BoxId
    ,button.ShapeIndex as ShapeIndex
    ,button.GraphicIndex as GraphicIndex
    ,button.TextTypeIndex as TextTypeIndex
    ,button.OrientationIndex as OrientationIndex
    ,button.IconId as IconId
    ,button.ImageId as ImageId
from [Content].[Button] button
where 1=1
    and button.IsDeleted = 0
    and button.VersionOf = button.Id