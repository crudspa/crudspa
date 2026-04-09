create proc [EducationPublisher].[BookInsert] (
     @SessionId uniqueidentifier
    ,@Title nvarchar(150)
    ,@StatusId uniqueidentifier
    ,@Key nvarchar(100)
    ,@Author nvarchar(150)
    ,@Isbn nvarchar(20)
    ,@Lexile nvarchar(10)
    ,@SeasonId uniqueidentifier
    ,@CategoryId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@Summary nvarchar(max)
    ,@CoverImageId uniqueidentifier
    ,@GuideImageId uniqueidentifier
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

insert [Education].[Book] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Title
    ,ContentStatusId
    ,[Key]
    ,Author
    ,Isbn
    ,Lexile
    ,SeasonId
    ,CategoryId
    ,RequiresAchievementId
    ,Summary
    ,CoverImageId
    ,GuideImageId
    ,OwnerId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Title
    ,@StatusId
    ,@Key
    ,@Author
    ,@Isbn
    ,@Lexile
    ,@SeasonId
    ,@CategoryId
    ,@RequiresAchievementId
    ,@Summary
    ,@CoverImageId
    ,@GuideImageId
    ,@organizationId
)

if not exists (
    select 1
    from [Education].[Book-Active] book
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    where book.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction