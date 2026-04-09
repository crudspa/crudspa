create view [Content].[MultimediaItem-Active] as

select multimediaItem.Id as Id
    ,multimediaItem.MultimediaElementId as MultimediaElementId
    ,multimediaItem.BoxId as BoxId
    ,multimediaItem.ItemId as ItemId
    ,multimediaItem.MediaTypeIndex as MediaTypeIndex
    ,multimediaItem.AudioId as AudioId
    ,multimediaItem.ButtonId as ButtonId
    ,multimediaItem.ImageId as ImageId
    ,multimediaItem.Text as Text
    ,multimediaItem.VideoId as VideoId
    ,multimediaItem.Ordinal as Ordinal
from [Content].[MultimediaItem] multimediaItem
where 1=1
    and multimediaItem.IsDeleted = 0
    and multimediaItem.VersionOf = multimediaItem.Id