create trigger [Content].[MultimediaItemTrigger] on [Content].[MultimediaItem]
    for update
as

insert [Content].[MultimediaItem] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,MultimediaElementId
    ,BoxId
    ,ItemId
    ,MediaTypeIndex
    ,AudioId
    ,ButtonId
    ,ImageId
    ,Text
    ,VideoId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.MultimediaElementId
    ,deleted.BoxId
    ,deleted.ItemId
    ,deleted.MediaTypeIndex
    ,deleted.AudioId
    ,deleted.ButtonId
    ,deleted.ImageId
    ,deleted.Text
    ,deleted.VideoId
    ,deleted.Ordinal
from deleted