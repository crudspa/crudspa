create proc [EducationPublisher].[BookSelectNames] as

set nocount on
select
    book.Id
    ,book.[Key] + ' - ' + book.Title as Name
from [Education].[Book-Active] book
order by book.[Key], book.Title