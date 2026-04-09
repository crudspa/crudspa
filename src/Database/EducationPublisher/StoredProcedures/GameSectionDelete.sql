create proc [EducationPublisher].[GameSectionDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @gameId uniqueidentifier = (select top 1 GameId from [Education].[GameSection] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[GameSection-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[GameSection-Active] gameSection
        inner join [Education].[Game-Active] game on gameSection.GameId = game.Id
        inner join [Education].[Book-Active] book on game.BookId = book.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    where gameSection.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update gameSection
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[GameSection] gameSection
where gameSection.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[GameSection] baseTable
    inner join [Education].[GameSection-Active] gameSection on gameSection.Id = baseTable.Id
where gameSection.GameId = @gameId
    and gameSection.Ordinal > @oldOrdinal

commit transaction