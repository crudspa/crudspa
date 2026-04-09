create proc [EducationPublisher].[GameActivityDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @sectionId uniqueidentifier = (select top 1 SectionId from [Education].[GameActivity] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[GameActivity-Active] where Id = @Id)
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
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[GameActivity] gameActivity
where gameActivity.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[GameActivity] baseTable
    inner join [Education].[GameActivity-Active] gameActivity on gameActivity.Id = baseTable.Id
where gameActivity.SectionId = @sectionId
    and gameActivity.Ordinal > @oldOrdinal

commit transaction