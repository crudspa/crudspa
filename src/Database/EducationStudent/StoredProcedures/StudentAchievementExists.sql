create proc [EducationStudent].[StudentAchievementExists] (
     @StudentId uniqueidentifier
    ,@AchievementId uniqueidentifier
    ,@AlreadyUnlocked bit output
) as

if (exists(
    select
        studentAchievement.Id as Id
    from [Education].[StudentAchievement-Active] studentAchievement
    where studentAchievement.StudentId = @StudentId
        and studentAchievement.AchievementId = @AchievementId
))
begin
    set @AlreadyUnlocked = 1
end
else
begin
    set @AlreadyUnlocked = 0
end