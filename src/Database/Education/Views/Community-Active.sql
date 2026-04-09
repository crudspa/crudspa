create view [Education].[Community-Active] as

select community.Id as Id
    ,community.DistrictId as DistrictId
    ,community.Name as Name
from [Education].[Community] community
where 1=1
    and community.IsDeleted = 0
    and community.VersionOf = community.Id