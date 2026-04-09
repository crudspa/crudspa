create proc [EducationPublisher].[GameActivityUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@ThemeWord nvarchar(50)
    ,@GroupId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Rigorous bit
    ,@Multisyllabic bit
    ,@ActivityId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[GameActivity-Active] gameActivity
        inner join [Education].[GameSection-Active] gameSection on gameActivity.SectionId = gameSection.Id
        inner join [Education].[Game-Active] game on gameSection.GameId = game.Id
        inner join [Education].[Book-Active] book on game.BookId = book.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    where gameActivity.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update gameActivity
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,ThemeWord = @ThemeWord
    ,GroupId = @GroupId
    ,TypeId = @TypeId
    ,Rigorous = @Rigorous
    ,Multisyllabic = @Multisyllabic
    ,ActivityId = @ActivityId
from [Education].[GameActivity] gameActivity
where gameActivity.Id = @Id

commit transaction