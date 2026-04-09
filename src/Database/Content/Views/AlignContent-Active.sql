create view [Content].[AlignContent-Active] as

select alignContent.Id as Id
    ,alignContent.Name as Name
    ,alignContent.Ordinal as Ordinal
from [Content].[AlignContent] alignContent
where 1=1
    and alignContent.IsDeleted = 0