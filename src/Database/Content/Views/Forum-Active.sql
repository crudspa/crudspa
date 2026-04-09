create view [Content].[Forum-Active] as

select forum.Id as Id
    ,forum.PortalId as PortalId
    ,forum.StatusId as StatusId
    ,forum.Title as Title
    ,forum.Description as Description
    ,forum.ImageId as ImageId
    ,forum.Ordinal as Ordinal
from [Content].[Forum] forum
where 1=1
    and forum.IsDeleted = 0
    and forum.VersionOf = forum.Id