create proc [EducationPublisher].[BookSelectNamesWithGames] as

select distinct
    book.Id
    ,book.[Key] + ' - ' + book.Title as Name
from [Education].[Book-Active] as book
    inner join [Education].[Game-Active] game on game.BookId = book.Id