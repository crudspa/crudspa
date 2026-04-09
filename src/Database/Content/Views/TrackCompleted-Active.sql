create view [Content].[TrackCompleted-Active] as

select trackCompleted.Id as Id
    ,trackCompleted.ContactId as ContactId
    ,trackCompleted.TrackId as TrackId
    ,trackCompleted.Completed as Completed
from [Content].[TrackCompleted] trackCompleted
where 1=1
    and trackCompleted.IsDeleted = 0