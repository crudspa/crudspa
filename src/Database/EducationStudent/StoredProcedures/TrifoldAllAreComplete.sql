create proc [EducationStudent].[TrifoldAllAreComplete] (
     @SessionId uniqueidentifier
    ,@TrifoldId uniqueidentifier
    ,@AllAreComplete bit output
) as

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @bookId uniqueidentifier = (select BookId from [Education].[Trifold-Active] where Id = @TrifoldId)

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

if (exists(
    select trifold.Id
    from [Education].[Trifold-Active] trifold
    where trifold.BookId = @bookId
        and trifold.StatusId = @ContentStatusComplete
        and trifold.Id not in (
            select TrifoldId from [Education].[TrifoldCompleted] where StudentId = @studentId
        )
        and (trifold.RequiresAchievementId is null
            or exists (
                select Id
                from [Education].[StudentAchievement-Active]
                where StudentId = @studentId
                    and AchievementId = trifold.RequiresAchievementId
            )
        )
))
begin
    set @AllAreComplete = 0
end
else
begin
    set @AllAreComplete = 1
end