merge into [Content].[Population] as Target
using ( values
     ('87b8c9da-1e2f-4031-9456-789abcdef401', '73410fd3-3681-46d3-800e-a08670e291cf', 'sample-users', 'Sample Portal Users', 'Active users in the target organization and portal.', 0, 'organization-users')
) as Source
    (Id, PortalId, [Key], Name, Description, SupportsOptOut, ResolverKey)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.PortalId = Source.PortalId
    ,Target.[Key] = Source.[Key]
    ,Target.Name = Source.Name
    ,Target.Description = Source.Description
    ,Target.SupportsOptOut = Source.SupportsOptOut
    ,Target.ResolverKey = Source.ResolverKey

when not matched by target then
insert (Id, PortalId, [Key], Name, Description, SupportsOptOut, ResolverKey)
values (Id, PortalId, [Key], Name, Description, SupportsOptOut, ResolverKey)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;