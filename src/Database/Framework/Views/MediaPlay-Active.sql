create view [Framework].[MediaPlay-Active] as

select mediaPlay.Id as Id
    ,mediaPlay.AudioFileId as AudioFileId
    ,mediaPlay.VideoFileId as VideoFileId
    ,mediaPlay.Started as Started
    ,mediaPlay.Canceled as Canceled
    ,mediaPlay.Completed as Completed
from [Framework].[MediaPlay] mediaPlay
where 1=1
    and mediaPlay.IsDeleted = 0
    and mediaPlay.VersionOf = mediaPlay.Id