create view [Education].[Module-Active] as

select module.Id as Id
    ,module.Title as Title
    ,module.IconId as IconId
    ,module.IconName as IconName
    ,module.BookId as BookId
    ,module.StatusId as StatusId
    ,module.BinderId as BinderId
    ,module.RequiresAchievementId as RequiresAchievementId
    ,module.GeneratesAchievementId as GeneratesAchievementId
    ,module.Ordinal as Ordinal
from [Education].[Module] module
where 1=1
    and module.IsDeleted = 0
    and module.VersionOf = module.Id