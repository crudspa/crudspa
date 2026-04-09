create proc [EducationPublisher].[GameActivitySelectForSection] (
     @SessionId uniqueidentifier
    ,@SectionId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     gameActivity.Id
    ,gameActivity.SectionId
    ,section.Title as SectionTitle
    ,gameActivity.ThemeWord
    ,gameActivity.GroupId
    ,groupTable.Name as GroupName
    ,gameActivity.TypeId
    ,type.Name as TypeName
    ,gameActivity.Rigorous
    ,gameActivity.Multisyllabic
    ,gameActivity.ActivityId
    ,activity.[Key] as ActivityKey
    ,gameActivity.Ordinal
from [Education].[GameActivity-Active] gameActivity
    inner join [Education].[Activity-Active] activity on gameActivity.ActivityId = activity.Id
    inner join [Education].[GameSection-Active] gameSection on gameActivity.SectionId = gameSection.Id
    inner join [Education].[Game-Active] game on gameSection.GameId = game.Id
    inner join [Education].[Book-Active] book on game.BookId = book.Id
    left join [Education].[ResearchGroup-Active] groupTable on gameActivity.GroupId = groupTable.Id
    inner join [Framework].[Organization-Active] organization on book.OwnerId = organization.Id
    inner join [Education].[GameSection-Active] section on gameActivity.SectionId = section.Id
    inner join [Education].[GameActivityType-Active] type on gameActivity.TypeId = type.Id
where gameActivity.SectionId = @SectionId
    and organization.Id = @organizationId

select
    activity.Id
    ,activity.[Key]
    ,activity.ActivityTypeId
    ,activity.ContentAreaId
    ,activity.ContextText
    ,contextAudioFile.Id as ContextAudioFileId
    ,contextAudioFile.BlobId as ContextAudioFileBlobId
    ,contextAudioFile.Name as ContextAudioFileName
    ,contextAudioFile.Format as ContextAudioFileFormat
    ,contextAudioFile.OptimizedStatus as ContextAudioFileOptimizedStatus
    ,contextAudioFile.OptimizedBlobId as ContextAudioFileOptimizedBlobId
    ,contextAudioFile.OptimizedFormat as ContextAudioFileOptimizedFormat
    ,contextImageFile.Id as ContextImageFileId
    ,contextImageFile.BlobId as ContextImageFileBlobId
    ,contextImageFile.Name as ContextImageFileName
    ,contextImageFile.Format as ContextImageFileFormat
    ,contextImageFile.Width as ContextImageFileWidth
    ,contextImageFile.Height as ContextImageFileHeight
    ,contextImageFile.Caption as ContextImageFileCaption
    ,activity.ExtraText
    ,activityType.Name as ActivityTypeName
    ,activityType.DisplayView as ActivityTypeDisplayView
    ,category.Name as ActivityTypeCategoryName
    ,contentArea.Name as ContentAreaName
from [Education].[GameActivity-Active] gameActivity
    inner join [Education].[Activity-Active] activity on gameActivity.ActivityId = activity.Id
    left join [Framework].[AudioFile-Active] contextAudioFile on activity.ContextAudioFileId = contextAudioFile.Id
    left join [Framework].[ImageFile-Active] contextImageFile on activity.ContextImageFileId = contextImageFile.Id
    inner join [Education].[ActivityType-Active] activityType on activity.ActivityTypeId = activityType.Id
    inner join [Education].[ActivityCategory-Active] category on activityType.CategoryId = category.Id
    inner join [Education].[ContentArea-Active] contentArea on activity.ContentAreaId = contentArea.Id
where gameActivity.SectionId = @SectionId

select
    activityChoice.*
from [Education].[ActivityChoice-Deep] activityChoice
    inner join [Education].[Activity-Active] activity on activityChoice.ActivityId = activity.Id
    inner join [Education].[GameActivity-Active] gameActivity on gameActivity.ActivityId = activity.Id
where gameActivity.SectionId = @SectionId

select
    gameActivity.Id
    ,gameActivity.ActivityId
    ,section.Id as SectionId
    ,section.GameId as SectionGameId
    ,section.Title as SectionTitle
    ,sectionGame.[Key] as SectionGameKey
    ,sectionGameBook.Id as SectionGameBookId
    ,sectionGameBook.Title as SectionGameBookTitle
from [Education].[GameActivity-Active] gameActivity
    inner join [Education].[GameSection-Active] section on gameActivity.SectionId = section.Id
    inner join [Education].[Game-Active] sectionGame on section.GameId = sectionGame.Id
    inner join [Education].[Book-Active] sectionGameBook on sectionGame.BookId = sectionGameBook.Id
where gameActivity.SectionId != @SectionId
    and gameActivity.ActivityId in (
        select ActivityId
        from [Education].[GameActivity-Active]
        where SectionId = @SectionId
    )