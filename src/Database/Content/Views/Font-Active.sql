create view [Content].[Font-Active] as

select font.Id as Id
    ,font.Name as Name
    ,font.ContentPortalId as ContentPortalId
    ,font.FileId as FileId
from [Content].[Font] font
where 1=1
    and font.IsDeleted = 0
    and font.VersionOf = font.Id