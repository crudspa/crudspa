create proc [EducationPublisher].[UnitInsert] (
     @SessionId uniqueidentifier
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

declare @ordinal int = (select count(1) from [Education].[Unit-Active])

insert [Education].[Unit] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Title
    ,StatusId
    ,GradeId
    ,ParentId
    ,RequiresAchievementId
    ,GeneratesAchievementId
    ,ImageId
    ,GuideText
    ,GuideAudioId
    ,IntroAudioId
    ,SongAudioId
    ,Treatment
    ,Control
    ,OwnerId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Title
    ,@StatusId
    ,@GradeId
    ,@ParentId
    ,@RequiresAchievementId
    ,@GeneratesAchievementId
    ,@ImageId
    ,@GuideText
    ,@GuideAudioId
    ,@IntroAudioId
    ,@SongAudioId
    ,@Treatment
    ,@Control
    ,@organizationId
    ,@ordinal
)

if not exists (
    select 1
    from [Education].[Unit-Active] unit
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where unit.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction