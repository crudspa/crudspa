create trigger [Education].[PublisherContactTrigger] on [Education].[PublisherContact]
    for update
as

insert [Education].[PublisherContact] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PublisherId
    ,ContactId
    ,UserId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PublisherId
    ,deleted.ContactId
    ,deleted.UserId
from deleted