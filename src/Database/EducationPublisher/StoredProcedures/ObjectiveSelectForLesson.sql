create proc [EducationPublisher].[ObjectiveSelectForLesson] (
     @SessionId uniqueidentifier
    ,@LessonId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     objective.Id
    ,objective.LessonId
    ,lesson.Title as LessonTitle
    ,objective.Title
    ,objective.StatusId
    ,status.Name as StatusName
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
    ,objective.RequiresAchievementId
    ,requiresAchievement.Title as RequiresAchievementTitle
    ,objective.GeneratesAchievementId
    ,generatesAchievement.Title as GeneratesAchievementTitle
    ,objective.Ordinal
    ,binder.Id as BinderId
    ,binder.TypeId as BinderTypeId
    ,type.Name as BinderTypeName
    ,(select count(1) from [Content].[Page-Active] where BinderId = objective.BinderId) as PageCount
from [Education].[Objective-Active] objective
    inner join [Content].[Binder-Active] binder on objective.BinderId = binder.Id
    left join [Education].[Achievement-Active] generatesAchievement on objective.GeneratesAchievementId = generatesAchievement.Id
    inner join [Education].[Lesson-Active] lesson on objective.LessonId = lesson.Id
    left join [Education].[Achievement-Active] requiresAchievement on objective.RequiresAchievementId = requiresAchievement.Id
    inner join [Framework].[ContentStatus-Active] status on objective.StatusId = status.Id
    inner join [Framework].[ImageFile-Active] trophyImage on objective.TrophyImageId = trophyImage.Id
    inner join [Content].[BinderType-Active] type on binder.TypeId = type.Id
    inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
where objective.LessonId = @LessonId
    and organization.Id = @organizationId