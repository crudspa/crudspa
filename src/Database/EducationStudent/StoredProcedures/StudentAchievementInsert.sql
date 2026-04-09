create proc [EducationStudent].[StudentAchievementInsert] (
     @SessionId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@AchievementId uniqueidentifier
    ,@RelatedEntityId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    insert [Education].[StudentAchievement] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,StudentId
        ,AchievementId
        ,RelatedEntityId
        ,Earned
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@StudentId
        ,@AchievementId
        ,@RelatedEntityId
        ,@now
    )

commit transaction