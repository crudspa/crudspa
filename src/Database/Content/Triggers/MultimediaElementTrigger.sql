create trigger [Content].[MultimediaElementTrigger] on [Content].[MultimediaElement]
    for update
as

insert [Content].[MultimediaElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,ContainerId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.ContainerId
from deleted