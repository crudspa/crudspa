create proc [EducationStudent].[GameProgressSelectAll] (
     @SessionId uniqueidentifier
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

;with GamesCompletedCte(GameId, EventCount) as (
    select gameCompleted.GameId, count(1) as GameCompletedCount
    from [Education].[GameCompleted] gameCompleted
    where gameCompleted.StudentId = @studentId
    group by gameCompleted.GameId
)

select
    @StudentId as StudentId
    ,game.Id as GameId
    ,game.BookId as BookId
    ,isnull(gamesCompleted.EventCount, 0) as TimesCompleted
from [Education].[Game-Active] game
    left join GamesCompletedCte gamesCompleted on game.Id = gamesCompleted.GameId
where gamesCompleted.EventCount is not null