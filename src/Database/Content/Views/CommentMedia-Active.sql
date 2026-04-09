create view [Content].[CommentMedia-Active] as

select commentMedia.Id as Id
    ,commentMedia.CommentId as CommentId
    ,commentMedia.Type as Type
    ,commentMedia.AudioId as AudioId
    ,commentMedia.ImageId as ImageId
    ,commentMedia.PdfId as PdfId
    ,commentMedia.VideoId as VideoId
    ,commentMedia.Ordinal as Ordinal
from [Content].[CommentMedia] commentMedia
where 1=1
    and commentMedia.IsDeleted = 0
    and commentMedia.VersionOf = commentMedia.Id