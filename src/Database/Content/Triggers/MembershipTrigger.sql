create trigger [Content].[MembershipTrigger] on [Content].[Membership]
    for update
as

insert [Content].[Membership] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,Name
    ,Description
    ,SupportsOptOut
    ,PopulationId
    ,OrganizationId
    ,ActivationScopeId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.Name
    ,deleted.Description
    ,deleted.SupportsOptOut
    ,deleted.PopulationId
    ,deleted.OrganizationId
    ,deleted.ActivationScopeId
from deleted