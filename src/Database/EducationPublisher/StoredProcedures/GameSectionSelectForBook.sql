create proc [EducationPublisher].[GameSectionSelectForBook] (
     @BookId uniqueidentifier
) as

select
    gameSection.Id as Id
    ,(game.[Key] + ' - ' + gameSection.Title) as Name
from [Education].[GameSection-Active] gameSection
    inner join [Education].[Game-Active] game on gameSection.GameId = game.Id
where game.BookId = @BookId
order by game.[Key], gameSection.Ordinal