create view [Framework].[AccessCode-Active] as

select accessCode.Id as Id
    ,accessCode.Created as Created
    ,accessCode.SessionId as SessionId
    ,accessCode.UserId as UserId
    ,accessCode.PortalId as PortalId
    ,accessCode.Code as Code
    ,accessCode.Expires as Expires
    ,accessCode.Used as Used
from [Framework].[AccessCode] accessCode
where 1=1