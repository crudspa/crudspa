create proc [EducationPublisher].[CommunitySelectNames] as

select
    community.Id
    ,community.Name
from [Education].[Community-Active] community
order by community.Name