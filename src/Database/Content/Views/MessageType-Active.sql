create view [Content].[MessageType-Active] as

select messageType.Id as Id
    ,messageType.Name as Name
    ,messageType.Ordinal as Ordinal
from [Content].[MessageType] messageType
where 1=1
    and messageType.IsDeleted = 0