create view [Framework].[Expression-Active] as

select expression.Id as Id
from [Framework].[Expression] expression
where 1=1
    and expression.IsDeleted = 0
    and expression.VersionOf = expression.Id