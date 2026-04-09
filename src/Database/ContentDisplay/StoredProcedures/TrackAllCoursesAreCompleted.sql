create proc [ContentDisplay].[TrackAllCoursesAreCompleted] (
     @SessionId uniqueidentifier
    ,@TrackId uniqueidentifier
    ,@CourseId uniqueidentifier
    ,@AllAreComplete bit output
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

declare @contactId uniqueidentifier = (
    select contact.Id
    from [Framework].[Contact-Active] contact
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

if (@TrackId is null and @CourseId is not null)
begin
    set @TrackId = (select top 1 TrackId from [Content].[Course-Active] where Id = @CourseId)
end

if (exists (
    select track.Id
    from [Content].[Track-Active] track
        inner join [Content].[Course-Active] course on course.TrackId = track.Id
    where track.Id = @TrackId
        and course.Id not in (select CourseId from [Content].[CourseCompleted] where ContactId = @contactId)
        and course.StatusId = @ContentStatusComplete
        and (course.RequiresAchievementId is null
            or exists (
                select Id
                from [Content].[ContactAchievement-Active]
                where ContactId = @contactId
                    and AchievementId = course.RequiresAchievementId
            )
        )
    )
)
begin
    set @AllAreComplete = 0
end
else
begin
    set @AllAreComplete = 1
end