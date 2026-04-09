create view [Education].[Activity-Active] as

select activity.Id as Id
    ,activity.[Key] as [Key]
    ,activity.ActivityTypeId as ActivityTypeId
    ,activity.ContentAreaId as ContentAreaId
    ,activity.StatusId as StatusId
    ,activity.ContextText as ContextText
    ,activity.ContextAudioFileId as ContextAudioFileId
    ,activity.ContextImageFileId as ContextImageFileId
    ,activity.ExtraText as ExtraText
from [Education].[Activity] activity
where 1=1
    and activity.IsDeleted = 0
    and activity.VersionOf = activity.Id