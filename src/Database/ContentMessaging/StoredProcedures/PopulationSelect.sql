create proc [ContentMessaging].[PopulationSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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
where population.Id = @Id