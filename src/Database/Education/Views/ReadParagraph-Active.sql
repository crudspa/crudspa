create view [Education].[ReadParagraph-Active] as

select readParagraph.Id as Id
    ,readParagraph.ReadPartId as ReadPartId
    ,readParagraph.Text as Text
    ,readParagraph.Ordinal as Ordinal
from [Education].[ReadParagraph] readParagraph
where 1=1
    and readParagraph.IsDeleted = 0
    and readParagraph.VersionOf = readParagraph.Id