create view [Education].[BookActivity-Active] as

select bookActivity.Id as Id
    ,bookActivity.BookId as BookId
    ,bookActivity.ActivityId as ActivityId
from [Education].[BookActivity] bookActivity
where 1=1
    and bookActivity.IsDeleted = 0
    and bookActivity.VersionOf = bookActivity.Id