create proc [ContentDisplay].[ContactAchievementInsert] (
     @SessionId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@AchievementId uniqueidentifier
    ,@RelatedEntityId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    insert [Content].[ContactAchievement] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,ContactId
        ,AchievementId
        ,RelatedEntityId
        ,Earned
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@ContactId
        ,@AchievementId
        ,@RelatedEntityId
        ,@now
    )

commit transaction