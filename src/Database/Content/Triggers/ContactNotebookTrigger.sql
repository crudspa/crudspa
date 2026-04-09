create trigger [Content].[ContactNotebookTrigger] on [Content].[ContactNotebook]
    for update
as

insert [Content].[ContactNotebook] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ContactId
    ,NotebookId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ContactId
    ,deleted.NotebookId
from deleted