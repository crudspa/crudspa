create proc [EducationStudent].[GameExternalSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Education].[GameViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,GameId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

select
    game.Id as Id
    ,game.BookId as BookId
    ,game.[Key] as [Key]
    ,game.Title as Title
    ,game.IconName as IconName
    ,book.Title as BookTitle
from [Education].[Game-Active] game
    inner join [Education].[Book-Active] book on game.BookId = book.Id
where game.Id = @Id
    and game.StatusId = @ContentStatusComplete