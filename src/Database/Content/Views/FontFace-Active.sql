create view [Content].[FontFace-Active] as

select fontFace.Id as Id
    ,fontFace.FontId as FontId
    ,fontFace.FileId as FileId
    ,fontFace.Style as Style
    ,fontFace.WeightMin as WeightMin
    ,fontFace.WeightMax as WeightMax
from [Content].[FontFace] fontFace
where 1=1
    and fontFace.IsDeleted = 0
    and fontFace.VersionOf = fontFace.Id