create trigger [Education].[RosterLinkTrigger] on [Education].[RosterLink]
    for update
as

insert [Education].[RosterLink] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,RosterSourceId
    ,Kind
    ,ExternalId
    ,LocalId
    ,SourceHash
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.RosterSourceId
    ,deleted.Kind
    ,deleted.ExternalId
    ,deleted.LocalId
    ,deleted.SourceHash
from deleted