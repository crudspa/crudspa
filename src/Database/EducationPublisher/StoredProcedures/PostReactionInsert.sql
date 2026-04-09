create proc [EducationPublisher].[PostReactionInsert] (
     @SessionId uniqueidentifier
    ,@PostId uniqueidentifier
    ,@Character nvarchar(2)
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

insert [Education].[PostReaction] (
    Id
    ,Updated
    ,UpdatedBy
    ,PostId
    ,ById
    ,Character
    ,Reacted
)
values (
    newid()
    ,@now
    ,@SessionId
    ,@PostId
    ,@contactId
    ,@Character
    ,@now
)