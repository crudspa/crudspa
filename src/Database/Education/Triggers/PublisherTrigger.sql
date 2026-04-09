create trigger [Education].[PublisherTrigger] on [Education].[Publisher]
    for update
as

insert [Education].[Publisher] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,OrganizationId
    ,ProviderId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.OrganizationId
    ,deleted.ProviderId
from deleted