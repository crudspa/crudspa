create view [Education].[ActivityType-Active] as

select activityType.Id as Id
    ,activityType.Name as Name
    ,activityType.[Key] as [Key]
    ,activityType.DisplayView as DisplayView
    ,activityType.CategoryId as CategoryId
    ,activityType.StatusId as StatusId
    ,activityType.ContextGuidance as ContextGuidance
    ,activityType.ChoiceGuidance as ChoiceGuidance
    ,activityType.ExtraGuidance as ExtraGuidance
    ,activityType.SupportsContextText as SupportsContextText
    ,activityType.RequiresContextText as RequiresContextText
    ,activityType.SupportsContextAudio as SupportsContextAudio
    ,activityType.RequiresContextAudio as RequiresContextAudio
    ,activityType.SupportsContextImage as SupportsContextImage
    ,activityType.RequiresContextImage as RequiresContextImage
    ,activityType.SupportsExtraText as SupportsExtraText
    ,activityType.RequiresExtraText as RequiresExtraText
    ,activityType.SupportsChoices as SupportsChoices
    ,activityType.RequiresCorrectChoices as RequiresCorrectChoices
    ,activityType.SupportsAudioChoices as SupportsAudioChoices
    ,activityType.RequiresAudioChoices as RequiresAudioChoices
    ,activityType.RequiresDataChoices as RequiresDataChoices
    ,activityType.RequiresImageChoices as RequiresImageChoices
    ,activityType.SupportsTextChoices as SupportsTextChoices
    ,activityType.RequiresTextChoices as RequiresTextChoices
    ,activityType.RequiresLongerTextChoices as RequiresLongerTextChoices
    ,activityType.RequiresColumnOrdinal as RequiresColumnOrdinal
    ,activityType.RequiresTextOrImageChoices as RequiresTextOrImageChoices
    ,activityType.MinChoices as MinChoices
    ,activityType.MaxChoices as MaxChoices
    ,activityType.MinCorrectChoices as MinCorrectChoices
    ,activityType.MaxCorrectChoices as MaxCorrectChoices
    ,activityType.ShuffleChoices as ShuffleChoices
from [Education].[ActivityType] activityType
where 1=1
    and activityType.IsDeleted = 0