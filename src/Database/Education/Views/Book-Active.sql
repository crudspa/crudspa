create view [Education].[Book-Active] as

select book.Id as Id
    ,book.OwnerId as OwnerId
    ,book.[Key] as [Key]
    ,book.Isbn as Isbn
    ,book.Title as Title
    ,book.Author as Author
    ,book.Lexile as Lexile
    ,book.SeasonId as SeasonId
    ,coalesce(book.ContentStatusId, book.StatusId) as StatusId
    ,book.Summary as Summary
    ,book.CoverImageId as CoverImageId
    ,book.ShowOnline as ShowOnline
    ,book.YouTubeId as YouTubeId
    ,book.SongAudioFileId as SongAudioFileId
    ,book.CategoryId as CategoryId
    ,book.IntroVideoId as IntroVideoId
    ,book.ReadAloudVideoId as ReadAloudVideoId
    ,book.PrefaceBinderId as PrefaceBinderId
    ,book.GuideImageId as GuideImageId
    ,book.RequiresAchievementId as RequiresAchievementId
from [Education].[Book] book
where 1=1
    and book.IsDeleted = 0
    and book.VersionOf = book.Id