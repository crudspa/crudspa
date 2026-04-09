create view [Content].[JustifyContent-Active] as

select justifyContent.Id as Id
    ,justifyContent.Name as Name
    ,justifyContent.Ordinal as Ordinal
from [Content].[JustifyContent] justifyContent
where 1=1
    and justifyContent.IsDeleted = 0