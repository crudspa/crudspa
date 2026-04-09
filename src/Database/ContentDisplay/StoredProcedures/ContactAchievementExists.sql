create proc [ContentDisplay].[ContactAchievementExists] (
     @ContactId uniqueidentifier
    ,@AchievementId uniqueidentifier
    ,@AlreadyUnlocked bit output
) as

if (exists(
    select
        contactAchievement.Id as Id
    from [Content].[ContactAchievement-Active] contactAchievement
    where contactAchievement.ContactId = @ContactId
        and contactAchievement.AchievementId = @AchievementId
))
begin
    set @AlreadyUnlocked = 1
end
else
begin
    set @AlreadyUnlocked = 0
end