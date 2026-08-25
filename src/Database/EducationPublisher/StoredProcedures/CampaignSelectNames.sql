create proc [EducationPublisher].[CampaignSelectNames] (
     @SessionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     campaign.Id
    ,campaign.Name as Name
from [Content].[Campaign-Active] campaign
    inner join [Framework].[Portal-Active] portal on campaign.PortalId = portal.Id
where portal.OwnerId = @organizationId
order by campaign.Name