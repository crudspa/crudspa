create trigger [Education].[GuideBinderTrigger] on [Education].[GuideBinder]
    for update
as

insert [Education].[GuideBinder] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,BinderId
    ,GuideImageId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.BinderId
    ,deleted.GuideImageId
from deleted