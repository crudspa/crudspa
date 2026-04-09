create proc [EducationPublisher].[GameSectionInsert] (
     @SessionId uniqueidentifier
    ,@GameId uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Education].[GameSection-Active] where GameId = @GameId)

insert [Education].[GameSection] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,GameId
    ,Title
    ,StatusId
    ,TypeId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@GameId
    ,@Title
    ,@StatusId
    ,@TypeId
    ,@ordinal
)

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

commit transaction