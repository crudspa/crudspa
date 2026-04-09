create view [Framework].[Portal-Active] as

select portal.Id as Id
    ,portal.[Key] as [Key]
    ,portal.Title as Title
    ,portal.OwnerId as OwnerId
    ,portal.NavigationTypeId as NavigationTypeId
    ,portal.SessionsPersist as SessionsPersist
    ,portal.AllowSignIn as AllowSignIn
    ,portal.RequireSignIn as RequireSignIn
from [Framework].[Portal] portal
where 1=1
    and portal.IsDeleted = 0