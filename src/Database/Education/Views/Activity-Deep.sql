create view [Education].[Activity-Deep] as

select activity.Id
    ,activity.[Key]
    ,activity.ActivityTypeId
    ,activity.ContentAreaId
    ,activity.StatusId
    ,activity.ContextText
    ,activity.ContextAudioFileId
    ,activity.ContextImageFileId
    ,activity.ExtraText
    ,activityActivityType.Name as ActivityActivityTypeName
    ,activityActivityType.[Key] as ActivityActivityTypeKey
    ,activityActivityType.DisplayView as ActivityActivityTypeDisplayView
    ,activityActivityTypeCategory.[Key] as ActivityActivityTypeCategoryKey
    ,activityActivityTypeCategory.Name as ActivityActivityTypeCategoryName
    ,activityActivityType.ShuffleChoices as ActivityActivityTypeShuffleChoices
    ,activityContentArea.Name as ActivityContentAreaName
    ,activityContentArea.[Key] as ActivityContentAreaKey
    ,activityContentArea.AppNavText as ActivityContentAreaAppNavText
    ,activityContextAudioFile.Id as ActivityContextAudioFileId2
    ,activityContextAudioFile.BlobId as ActivityContextAudioFileBlobId
    ,activityContextAudioFile.Name as ActivityContextAudioFileName
    ,activityContextAudioFile.Format as ActivityContextAudioFileFormat
    ,activityContextAudioFile.OptimizedStatus as ActivityContextAudioFileOptimizedStatus
    ,activityContextAudioFile.OptimizedBlobId as ActivityContextAudioFileOptimizedBlobId
    ,activityContextAudioFile.OptimizedFormat as ActivityContextAudioFileOptimizedFormat
    ,activityContextImageFile.Id as ActivityContextImageFileId2
    ,activityContextImageFile.BlobId as ActivityContextImageFileBlobId
    ,activityContextImageFile.Name as ActivityContextImageFileName
    ,activityContextImageFile.Format as ActivityContextImageFileFormat
    ,activityContextImageFile.Width as ActivityContextImageFileWidth
    ,activityContextImageFile.Height as ActivityContextImageFileHeight
    ,activityContextImageFile.Caption as ActivityContextImageFileCaption
    ,activityStatus.Name as ActivityStatusName
from [Education].[Activity-Active] activity
    inner join [Education].[ActivityType-Active] activityActivityType on activity.ActivityTypeId = activityActivityType.Id
    inner join [Education].[ActivityCategory-Active] activityActivityTypeCategory on activityActivityType.CategoryId = activityActivityTypeCategory.Id
    inner join [Education].[ContentArea-Active] activityContentArea on activity.ContentAreaId = activityContentArea.Id
    left join [Framework].[AudioFile-Active] activityContextAudioFile on activity.ContextAudioFileId = activityContextAudioFile.Id
    left join [Framework].[ImageFile-Active] activityContextImageFile on activity.ContextImageFileId = activityContextImageFile.Id
    inner join [Education].[ActivityStatus-Active] activityStatus on activity.StatusId = activityStatus.Id