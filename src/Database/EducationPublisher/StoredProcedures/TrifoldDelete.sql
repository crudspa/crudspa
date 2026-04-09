create proc [EducationPublisher].[TrifoldDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @bookId uniqueidentifier = (select top 1 BookId from [Education].[Trifold] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[Trifold-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[Trifold-Active] trifold
        inner join [Education].[Book-Active] book on trifold.BookId = book.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    where trifold.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update trifold
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Trifold] trifold
where trifold.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[Trifold] baseTable
    inner join [Education].[Trifold-Active] trifold on trifold.Id = baseTable.Id
where trifold.BookId = @bookId
    and trifold.Ordinal > @oldOrdinal

commit transaction