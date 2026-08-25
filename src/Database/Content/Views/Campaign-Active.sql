create view [Content].[Campaign-Active] as

select campaign.Id as Id
    ,campaign.PortalId as PortalId
    ,campaign.Name as Name
    ,campaign.Description as Description
from [Content].[Campaign] campaign
where 1=1
    and campaign.IsDeleted = 0
    and campaign.VersionOf = campaign.Id