create view [Content].[Forum-Active] as

select forum.Id as Id
    ,forum.PortalId as PortalId
    ,forum.StatusId as StatusId
    ,forum.PermissionId as PermissionId
    ,forum.Title as Title
    ,forum.Description as Description
    ,forum.ImageId as ImageId
    ,forum.AccessMode as AccessMode
    ,forum.Ordinal as Ordinal
from [Content].[Forum] forum
where 1=1
    and forum.IsDeleted = 0
    and forum.VersionOf = forum.Id