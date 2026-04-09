create proc [EducationPublisher].[UnitUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@StatusId uniqueidentifier
    ,@GradeId uniqueidentifier
    ,@ParentId uniqueidentifier
    ,@RequiresAchievementId uniqueidentifier
    ,@GeneratesAchievementId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@GuideText nvarchar(max)
    ,@GuideAudioId uniqueidentifier
    ,@IntroAudioId uniqueidentifier
    ,@SongAudioId uniqueidentifier
    ,@Treatment bit
    ,@Control bit
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
    ,StatusId = @StatusId
    ,GradeId = @GradeId
    ,ParentId = @ParentId
    ,RequiresAchievementId = @RequiresAchievementId
    ,GeneratesAchievementId = @GeneratesAchievementId
    ,ImageId = @ImageId
    ,GuideText = @GuideText
    ,GuideAudioId = @GuideAudioId
    ,IntroAudioId = @IntroAudioId
    ,SongAudioId = @SongAudioId
    ,Treatment = @Treatment
    ,Control = @Control
from [Education].[Unit] baseTable
    inner join [Education].[Unit-Active] unit on unit.Id = baseTable.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction