create proc [EducationPublisher].[BookCategorySelectOrderables] as

set nocount on
select
    bookCategory.Id
    ,bookCategory.Name
    ,bookCategory.Ordinal
from [Education].[BookCategory-Active] bookCategory
order by bookCategory.Ordinal