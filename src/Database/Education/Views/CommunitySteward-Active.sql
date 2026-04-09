create view [Education].[CommunitySteward-Active] as

select communitySteward.Id as Id
    ,communitySteward.CommunityId as CommunityId
    ,communitySteward.DistrictContactId as DistrictContactId
from [Education].[CommunitySteward] communitySteward
where 1=1
    and communitySteward.IsDeleted = 0
    and communitySteward.VersionOf = communitySteward.Id