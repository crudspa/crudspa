create trigger [Framework].[SegmentTrigger] on [Framework].[Segment]
    for update
as

insert [Framework].[Segment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,[Key]
    ,Title
    ,StatusId
    ,Fixed
    ,RequiresId
    ,PortalId
    ,TypeId
    ,PermissionId
    ,IconId
    ,ParentId
    ,Recursive
    ,Vertical
    ,ConfigJson
    ,AllLicenses
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.[Key]
    ,deleted.Title
    ,deleted.StatusId
    ,deleted.Fixed
    ,deleted.RequiresId
    ,deleted.PortalId
    ,deleted.TypeId
    ,deleted.PermissionId
    ,deleted.IconId
    ,deleted.ParentId
    ,deleted.Recursive
    ,deleted.Vertical
    ,deleted.ConfigJson
    ,deleted.AllLicenses
    ,deleted.Ordinal
from deleted