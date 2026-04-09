create trigger [Education].[BookTrigger] on [Education].[Book]
    for update
as

insert [Education].[Book] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OwnerId
    ,[Key]
    ,Isbn
    ,Title
    ,Author
    ,Lexile
    ,SeasonId
    ,StatusId
    ,ContentStatusId
    ,Summary
    ,CoverImageId
    ,ShowOnline
    ,YouTubeId
    ,SongAudioFileId
    ,CategoryId
    ,IntroVideoId
    ,ReadAloudVideoId
    ,PrefaceBinderId
    ,GuideImageId
    ,RequiresAchievementId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OwnerId
    ,deleted.[Key]
    ,deleted.Isbn
    ,deleted.Title
    ,deleted.Author
    ,deleted.Lexile
    ,deleted.SeasonId
    ,deleted.StatusId
    ,deleted.ContentStatusId
    ,deleted.Summary
    ,deleted.CoverImageId
    ,deleted.ShowOnline
    ,deleted.YouTubeId
    ,deleted.SongAudioFileId
    ,deleted.CategoryId
    ,deleted.IntroVideoId
    ,deleted.ReadAloudVideoId
    ,deleted.PrefaceBinderId
    ,deleted.GuideImageId
    ,deleted.RequiresAchievementId
from deleted