create trigger [Education].[DistrictContactTrigger] on [Education].[DistrictContact]
    for update
as

insert [Education].[DistrictContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,DistrictId
    ,ContactId
    ,UserId
    ,Title
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.DistrictId
    ,deleted.ContactId
    ,deleted.UserId
    ,deleted.Title
from deleted