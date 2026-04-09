create proc [EducationPublisher].[GameActivityInsert] (
     @SessionId uniqueidentifier
    ,@SectionId uniqueidentifier
    ,@ThemeWord nvarchar(50)
    ,@GroupId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Rigorous bit
    ,@Multisyllabic bit
    ,@ActivityId uniqueidentifier
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

declare @ordinal int = (select count(1) from [Education].[GameActivity-Active] where SectionId = @SectionId)

insert [Education].[GameActivity] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SectionId
    ,ThemeWord
    ,GroupId
    ,TypeId
    ,Rigorous
    ,Multisyllabic
    ,ActivityId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@SectionId
    ,@ThemeWord
    ,@GroupId
    ,@TypeId
    ,@Rigorous
    ,@Multisyllabic
    ,@ActivityId
    ,@ordinal
)

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

commit transaction