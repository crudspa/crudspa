create view [Content].[Notebook-Active] as

select notebook.Id as Id
from [Content].[Notebook] notebook
where 1=1
    and notebook.IsDeleted = 0
    and notebook.VersionOf = notebook.Id