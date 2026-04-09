create proc [EducationPublisher].[ModuleInsert] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@IconId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
    ,@BinderTypeId uniqueidentifier
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

declare @ordinal int = (select count(1) from [Education].[Module-Active] where BookId = @BookId)

declare @binderId uniqueidentifier = newid()

insert [Content].[Binder] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,TypeId
)
values (
     @binderId
    ,@binderId
    ,@now
    ,@SessionId
    ,@BinderTypeId
)

insert [Education].[Module] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BookId
    ,Title
    ,StatusId
    ,IconId
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,BinderId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@BookId
    ,@Title
    ,@StatusId
    ,@IconId
    ,@RequiresAchievementId
    ,@GeneratesAchievementId
    ,@binderId
    ,@ordinal
)

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

commit transaction