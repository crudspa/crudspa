/*
merge into [Education].[Report] as Target
using ( values
-- todo: Add values
) as Source
    (Id, PortalId, IconId, DisplayView, Name, Description, PermissionId, Ordinal)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.PortalId = Source.PortalId
    ,Target.IconId = Source.IconId
    ,Target.DisplayView = Source.DisplayView
    ,Target.Name = Source.Name
    ,Target.Description = Source.Description
    ,Target.PermissionId = Source.PermissionId
    ,Target.Ordinal = Source.Ordinal

when not matched by target then
insert (Id, PortalId, IconId, DisplayView, Name, Description, PermissionId, Ordinal)
values (Id, PortalId, IconId, DisplayView, Name, Description, PermissionId, Ordinal)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/