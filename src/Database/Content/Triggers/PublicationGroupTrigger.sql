create trigger [Content].[PublicationGroupTrigger] on [Content].[PublicationGroup]
    for update
as

insert [Content].[PublicationGroup] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,Name
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.PortalId
    ,deleted.Name
    ,deleted.Ordinal
from deleted