create proc [ContentDisplay].[SurveyReplyComplete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
set xact_abort on
begin transaction

if (@contactId is not null)
begin
    update [Content].[SurveyReply]
    set Completed = @now
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where Id = @Id
        and ContactId = @contactId
        and SurveyId is not null
        and BinderId is null
        and Completed is null
        and VersionOf = Id
        and IsDeleted = 0
end

commit transaction