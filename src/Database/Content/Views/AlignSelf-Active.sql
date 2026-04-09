create view [Content].[AlignSelf-Active] as

select alignSelf.Id as Id
    ,alignSelf.Name as Name
    ,alignSelf.Ordinal as Ordinal
from [Content].[AlignSelf] alignSelf
where 1=1
    and alignSelf.IsDeleted = 0