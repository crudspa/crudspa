create view [Content].[Membership-Active] as

select membership.Id as Id
    ,membership.PortalId as PortalId
    ,membership.Name as Name
    ,membership.Description as Description
    ,membership.SupportsOptOut as SupportsOptOut
    ,membership.PopulationId as PopulationId
    ,membership.OrganizationId as OrganizationId
    ,membership.ActivationScopeId as ActivationScopeId
from [Content].[Membership] membership
where 1=1
    and membership.IsDeleted = 0
    and membership.VersionOf = membership.Id