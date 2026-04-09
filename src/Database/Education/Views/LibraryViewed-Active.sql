create view [Education].[LibraryViewed-Active] as

select libraryViewed.Id as Id
from [Education].[LibraryViewed] libraryViewed
where 1=1
    and libraryViewed.IsDeleted = 0