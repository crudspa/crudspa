create proc [EducationPublisher].[ModuleDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @bookId uniqueidentifier = (select top 1 BookId from [Education].[Module] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[Module-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[Module-Active] module
        inner join [Education].[Book-Active] book on module.BookId = book.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    where module.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update module
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Module] module
where module.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[Module] baseTable
    inner join [Education].[Module-Active] module on module.Id = baseTable.Id
where module.BookId = @bookId
    and module.Ordinal > @oldOrdinal

commit transaction