create proc [EducationPublisher].[AchievementInsert] (
     @SessionId uniqueidentifier
    ,@Title nvarchar(75)
    ,@RarityId uniqueidentifier
    ,@TrophyImageId uniqueidentifier
    ,@VisibleToStudents bit
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

insert [Education].[Achievement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Title
    ,RarityId
    ,TrophyImageId
    ,VisibleToStudents
    ,OwnerId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Title
    ,@RarityId
    ,@TrophyImageId
    ,@VisibleToStudents
    ,@organizationId
)

if not exists (
    select 1
    from [Education].[Achievement-Active] achievement
        inner join [Framework].[Organization-Active] organization on achievement.OwnerId = organization.Id
    where achievement.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction