create view [Content].[Track-Active] as

select track.Id as Id
    ,track.PortalId as PortalId
    ,track.Title as Title
    ,track.Description as Description
    ,track.StatusId as StatusId
    ,track.RequiresAchievementId as RequiresAchievementId
    ,track.GeneratesAchievementId as GeneratesAchievementId
    ,track.RequireSequentialCompletion as RequireSequentialCompletion
    ,track.Ordinal as Ordinal
from [Content].[Track] track
where 1=1
    and track.IsDeleted = 0
    and track.VersionOf = track.Id