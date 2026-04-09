create view [Content].[ContactNotebook-Active] as

select contactNotebook.Id as Id
    ,contactNotebook.ContactId as ContactId
    ,contactNotebook.NotebookId as NotebookId
from [Content].[ContactNotebook] contactNotebook
where 1=1
    and contactNotebook.IsDeleted = 0
    and contactNotebook.VersionOf = contactNotebook.Id