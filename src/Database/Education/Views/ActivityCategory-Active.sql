create view [Education].[ActivityCategory-Active] as

select activityCategory.Id as Id
    ,activityCategory.[Key] as [Key]
    ,activityCategory.Name as Name
from [Education].[ActivityCategory] activityCategory
where 1=1
    and activityCategory.IsDeleted = 0