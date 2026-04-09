create proc [EducationStudent].[TrifoldSelectForBook] (
     @BookId uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

declare @studentId uniqueidentifier = (
    select top 1 student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] users on users.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = users.Id
            and session.Id = @SessionId
)

insert [Education].[MapViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,BookId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@BookId
)

select
     trifold.Id as Id
    ,trifold.Title as Title
    ,trifold.BookId as BookId
    ,trifold.BinderId as BinderId
    ,trifold.Ordinal as Ordinal
    ,(select count(1) from [Content].[Page-Active] page where page.BinderId = trifold.BinderId) as PageCount
from [Education].[Trifold-Active] trifold
where trifold.BookId = @BookId
    and trifold.StatusId = @ContentStatusComplete
    and (trifold.RequiresAchievementId is null
        or exists (
            select Id
            from [Education].[StudentAchievement-Active]
            where StudentId = @studentId
                and AchievementId = trifold.RequiresAchievementId
        )
    )
order by trifold.Ordinal asc