create trigger [Education].[ReadParagraphTrigger] on [Education].[ReadParagraph]
    for update
as

insert [Education].[ReadParagraph] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,ReadPartId
    ,Text
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.ReadPartId
    ,deleted.Text
    ,deleted.Ordinal
from deleted