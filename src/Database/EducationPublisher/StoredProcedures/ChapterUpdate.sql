create proc [EducationPublisher].[ChapterUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@BinderTypeId uniqueidentifier
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

declare @binderId uniqueidentifier = (select top 1 BinderId from [Education].[Chapter] where Id = @Id)

update [Content].[Binder]
set
     Id = @binderId
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @BinderTypeId
where Id = @binderId

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
from [Education].[Chapter] baseTable
    inner join [Education].[Chapter-Active] chapter on chapter.Id = baseTable.Id
    inner join [Education].[Book-Active] book on chapter.BookId = book.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction