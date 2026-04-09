create view [Framework].[Session-Active] as

select session.Id as Id
    ,session.Started as Started
    ,session.Ended as Ended
    ,session.PortalId as PortalId
    ,session.UserId as UserId
    ,session.UserAdded as UserAdded
from [Framework].[Session] session
where 1=1
    and session.IsDeleted = 0