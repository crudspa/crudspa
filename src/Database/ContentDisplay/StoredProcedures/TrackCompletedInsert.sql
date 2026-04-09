create proc [ContentDisplay].[TrackCompletedInsert] (
     @SessionId uniqueidentifier
    ,@CourseId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @contactId uniqueidentifier = (
    select contact.Id
    from [Framework].[Contact-Active] contact
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @trackId uniqueidentifier = (select top 1 TrackId from [Content].[Course-Active] where Id = @CourseId)

if (not exists(select Id from [Content].[TrackCompleted-Active] where TrackId = @trackId and ContactId = @contactId))
begin
    insert [Content].[TrackCompleted] (
        Id
        ,UpdatedBy
        ,ContactId
        ,TrackId
        ,Completed
    )
    values (
        @Id
        ,@SessionId
        ,@contactId
        ,@trackId
        ,@now
    )
end