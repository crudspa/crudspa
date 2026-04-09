create view [Education].[Trifold-Active] as

select trifold.Id as Id
    ,trifold.Title as Title
    ,trifold.BookId as BookId
    ,trifold.StatusId as StatusId
    ,trifold.BinderId as BinderId
    ,trifold.RequiresAchievementId as RequiresAchievementId
    ,trifold.GeneratesAchievementId as GeneratesAchievementId
    ,trifold.Ordinal as Ordinal
from [Education].[Trifold] trifold
where 1=1
    and trifold.IsDeleted = 0
    and trifold.VersionOf = trifold.Id