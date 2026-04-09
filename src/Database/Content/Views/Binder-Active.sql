create view [Content].[Binder-Active] as

select binder.Id as Id
    ,binder.TypeId as TypeId
from [Content].[Binder] binder
where 1=1
    and binder.IsDeleted = 0
    and binder.VersionOf = binder.Id