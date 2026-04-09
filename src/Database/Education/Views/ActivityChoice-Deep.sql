create view [Education].[ActivityChoice-Deep] as

select distinct
    activityChoice.Id as Id
    ,activityChoice.ActivityId as ActivityId
    ,activityChoice.Text as Text
    ,activityChoice.AudioFileId as AudioFileId
    ,activityChoice.ImageFileId as ImageFileId
    ,activityChoice.IsCorrect as IsCorrect
    ,activityChoice.Ordinal as Ordinal
    ,activityChoice.ColumnOrdinal as ColumnOrdinal
    ,audioFile.Id as AudioFileId2
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,imageFile.Id as ImageFileId2
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
from [Education].[ActivityChoice-Active] activityChoice
    left join [Framework].[AudioFile-Active] audioFile on activityChoice.AudioFileId = audioFile.Id
    left join [Framework].[ImageFile-Active] imageFile on activityChoice.ImageFileId = imageFile.Id