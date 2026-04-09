create trigger [Education].[CommunityStewardTrigger] on [Education].[CommunitySteward]
    for update
as

insert [Education].[CommunitySteward] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,CommunityId
    ,DistrictContactId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.CommunityId
    ,deleted.DistrictContactId
from deleted