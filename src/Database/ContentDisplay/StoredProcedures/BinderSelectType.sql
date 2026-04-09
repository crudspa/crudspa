create proc [ContentDisplay].[BinderSelectType] (
     @BinderId uniqueidentifier
) as

select top 1
    binderType.Id
    ,binderType.Name
    ,binderType.DesignView
    ,binderType.DisplayView
from [Content].[BinderType-Active] binderType
    inner join [Content].[Binder-Active] binder on binder.TypeId = binderType.Id
where binder.Id = @BinderId