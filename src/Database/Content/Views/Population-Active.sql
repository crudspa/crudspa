create view [Content].[Population-Active] as

select population.Id as Id
    ,population.PortalId as PortalId
    ,population.[Key] as [Key]
    ,population.Name as Name
    ,population.Description as Description
    ,population.SupportsOptOut as SupportsOptOut
    ,population.ResolverKey as ResolverKey
from [Content].[Population] population
where 1=1
    and population.IsDeleted = 0