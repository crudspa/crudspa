create view [Content].[BinderType-Active] as

select binderType.Id as Id
    ,binderType.Name as Name
    ,binderType.DesignView as DesignView
    ,binderType.DisplayView as DisplayView
    ,binderType.Ordinal as Ordinal
from [Content].[BinderType] binderType
where 1=1
    and binderType.IsDeleted = 0