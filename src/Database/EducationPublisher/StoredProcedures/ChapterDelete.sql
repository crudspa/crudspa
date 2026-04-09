create proc [EducationPublisher].[ChapterDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @bookId uniqueidentifier = (select top 1 BookId from [Education].[Chapter] where Id = @Id)
declare @oldOrdinal int = (select top 1 Ordinal from [Education].[Chapter-Active] where Id = @Id)
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if not exists (
    select 1
    from [Education].[Chapter-Active] chapter
        inner join [Education].[Book-Active] book on chapter.BookId = book.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    where chapter.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update chapter
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Education].[Chapter] chapter
where chapter.Id = @Id

update baseTable
set baseTable.Ordinal = baseTable.Ordinal - 1
from [Education].[Chapter] baseTable
    inner join [Education].[Chapter-Active] chapter on chapter.Id = baseTable.Id
where chapter.BookId = @bookId
    and chapter.Ordinal > @oldOrdinal

commit transaction