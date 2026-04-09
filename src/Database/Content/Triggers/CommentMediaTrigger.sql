create trigger [Content].[CommentMediaTrigger] on [Content].[CommentMedia]
    for update
as

insert [Content].[CommentMedia] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,CommentId
    ,Type
    ,AudioId
    ,ImageId
    ,PdfId
    ,VideoId
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.CommentId
    ,deleted.Type
    ,deleted.AudioId
    ,deleted.ImageId
    ,deleted.PdfId
    ,deleted.VideoId
    ,deleted.Ordinal
from deleted