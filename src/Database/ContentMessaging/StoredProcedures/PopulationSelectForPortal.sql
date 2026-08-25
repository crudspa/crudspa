create proc [ContentMessaging].[PopulationSelectForPortal] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
) as

set nocount on

select
     population.Id
    ,population.PortalId
    ,population.[Key]
    ,population.Name
    ,population.Description
    ,population.SupportsOptOut
    ,population.ResolverKey
from [Content].[Population-Active] population
    inner join [Framework].[Portal-Active] portal on population.PortalId = portal.Id
    cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, population.PortalId)
where population.PortalId = @PortalId