create view [Framework].[AccessDenied-Active] as

select accessDenied.Id as Id
    ,accessDenied.Denied as Denied
    ,accessDenied.SessionId as SessionId
    ,accessDenied.EventType as EventType
    ,accessDenied.PermissionId as PermissionId
    ,accessDenied.Method as Method
from [Framework].[AccessDenied] accessDenied
where 1=1