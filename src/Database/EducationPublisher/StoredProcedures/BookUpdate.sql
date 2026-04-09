create proc [EducationPublisher].[BookUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,ContentStatusId = @StatusId
    ,[Key] = @Key
    ,Author = @Author
    ,Isbn = @Isbn
    ,Lexile = @Lexile
    ,SeasonId = @SeasonId
    ,CategoryId = @CategoryId
    ,RequiresAchievementId = @RequiresAchievementId
    ,Summary = @Summary
    ,CoverImageId = @CoverImageId
    ,GuideImageId = @GuideImageId
from [Education].[Book] baseTable
    inner join [Education].[Book-Active] book on book.Id = baseTable.Id
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