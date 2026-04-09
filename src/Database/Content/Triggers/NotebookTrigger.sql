create trigger [Content].[NotebookTrigger] on [Content].[Notebook]
    for update
as

insert [Content].[Notebook] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
from deleted