create view [Education].[ChapterViewed-Active] as

select chapterViewed.Id as Id
    ,chapterViewed.ChapterId as ChapterId
from [Education].[ChapterViewed] chapterViewed
where 1=1
    and chapterViewed.IsDeleted = 0