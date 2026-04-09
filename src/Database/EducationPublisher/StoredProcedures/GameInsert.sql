create proc [EducationPublisher].[GameInsert] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@Key nvarchar(75)
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@IconId uniqueidentifier
    ,@GradeId uniqueidentifier
    ,@AssessmentLevelId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
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

insert [Education].[Game] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BookId
    ,[Key]
    ,Title
    ,StatusId
    ,IconId
    ,GradeId
    ,AssessmentLevelId
    ,RequiresAchievementId
    ,GeneratesAchievementId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@BookId
    ,@Key
    ,@Title
    ,@StatusId
    ,@IconId
    ,@GradeId
    ,@AssessmentLevelId
    ,@RequiresAchievementId
    ,@GeneratesAchievementId
)

if not exists (
    select 1
    from [Education].[Game-Active] game
        inner join [Education].[Book-Active] book on game.BookId = book.Id
        inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    where game.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction