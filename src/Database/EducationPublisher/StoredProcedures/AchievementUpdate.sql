create proc [EducationPublisher].[AchievementUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@RarityId uniqueidentifier
    ,@TrophyImageId uniqueidentifier
    ,@VisibleToStudents bit
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
    ,RarityId = @RarityId
    ,TrophyImageId = @TrophyImageId
    ,VisibleToStudents = @VisibleToStudents
from [Education].[Achievement] baseTable
    inner join [Education].[Achievement-Active] achievement on achievement.Id = baseTable.Id
    inner join [Framework].[Organization-Active] organization on achievement.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction