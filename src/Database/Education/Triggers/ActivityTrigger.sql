create trigger [Education].[ActivityTrigger] on [Education].[Activity]
    for update
as

insert [Education].[Activity] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,[Key]
    ,ActivityTypeId
    ,ContentAreaId
    ,StatusId
    ,ContextText
    ,ContextAudioFileId
    ,ContextImageFileId
    ,ExtraText
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.[Key]
    ,deleted.ActivityTypeId
    ,deleted.ContentAreaId
    ,deleted.StatusId
    ,deleted.ContextText
    ,deleted.ContextAudioFileId
    ,deleted.ContextImageFileId
    ,deleted.ExtraText
from deleted