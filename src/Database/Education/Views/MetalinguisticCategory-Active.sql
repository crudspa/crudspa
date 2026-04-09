create view [Education].[MetalinguisticCategory-Active] as

select metalinguisticCategory.Id as Id
    ,metalinguisticCategory.Name as Name
    ,metalinguisticCategory.[Key] as [Key]
from [Education].[MetalinguisticCategory] metalinguisticCategory
where 1=1
    and metalinguisticCategory.IsDeleted = 0