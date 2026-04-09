create view [Content].[BinderCompleted-Active] as

select binderCompleted.Id as Id
    ,binderCompleted.ContactId as ContactId
    ,binderCompleted.BinderId as BinderId
    ,binderCompleted.DeviceTimestamp as DeviceTimestamp
from [Content].[BinderCompleted] binderCompleted
where 1=1
    and binderCompleted.IsDeleted = 0