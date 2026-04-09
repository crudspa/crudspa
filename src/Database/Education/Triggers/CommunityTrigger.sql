create trigger [Education].[CommunityTrigger] on [Education].[Community]
    for update
as

insert [Education].[Community] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,DistrictId
    ,Name
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.DistrictId
    ,deleted.Name
from deleted