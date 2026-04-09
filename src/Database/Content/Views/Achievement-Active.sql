create view [Content].[Achievement-Active] as

select achievement.Id as Id
    ,achievement.PortalId as PortalId
    ,achievement.Title as Title
    ,achievement.Description as Description
    ,achievement.ImageId as ImageId
from [Content].[Achievement] achievement
where 1=1
    and achievement.IsDeleted = 0
    and achievement.VersionOf = achievement.Id