create trigger [Content].[PdfElementTrigger] on [Content].[PdfElement]
    for update
as

insert [Content].[PdfElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ElementId
    ,FileId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ElementId
    ,deleted.FileId
from deleted