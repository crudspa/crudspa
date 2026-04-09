create view [Education].[Forum-Active] as

select forum.Id as Id
    ,forum.Name as Name
    ,forum.Description as Description
    ,forum.BodyTemplateId as BodyTemplateId
    ,forum.Pinned as Pinned
    ,forum.DistrictId as DistrictId
    ,forum.SchoolId as SchoolId
    ,forum.InnovatorsOnly as InnovatorsOnly
from [Education].[Forum] forum
where 1=1
    and forum.IsDeleted = 0
    and forum.VersionOf = forum.Id