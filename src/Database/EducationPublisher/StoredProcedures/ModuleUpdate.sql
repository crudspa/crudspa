create proc [EducationPublisher].[ModuleUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@IconId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
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

declare @binderId uniqueidentifier = (select top 1 BinderId from [Education].[Module] where Id = @Id)

update [Content].[Binder]
set
     Id = @binderId
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @BinderTypeId
where Id = @binderId

update module
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,StatusId = @StatusId
    ,IconId = @IconId
    ,RequiresAchievementId = @RequiresAchievementId
    ,GeneratesAchievementId = @GeneratesAchievementId
from [Education].[Module] module
where module.Id = @Id

commit transaction