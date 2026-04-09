create proc [EducationPublisher].[ActivityTypeSelectFull] as

set nocount on
select
     activityType.Id
    ,category.Name + ' - ' + activityType.Name as Name
    ,activityType.[Key]
    ,activityType.DisplayView
    ,activityType.CategoryId
    ,activityType.StatusId
    ,activityType.ContextGuidance
    ,activityType.ChoiceGuidance
    ,activityType.ExtraGuidance
    ,activityType.SupportsContextText
    ,activityType.RequiresContextText
    ,activityType.SupportsContextAudio
    ,activityType.RequiresContextAudio
    ,activityType.SupportsContextImage
    ,activityType.RequiresContextImage
    ,activityType.SupportsExtraText
    ,activityType.RequiresExtraText
    ,activityType.SupportsChoices
    ,activityType.RequiresCorrectChoices
    ,activityType.SupportsAudioChoices
    ,activityType.RequiresAudioChoices
    ,activityType.RequiresDataChoices
    ,activityType.RequiresImageChoices
    ,activityType.SupportsTextChoices
    ,activityType.RequiresTextChoices
    ,activityType.RequiresLongerTextChoices
    ,activityType.RequiresColumnOrdinal
    ,activityType.RequiresTextOrImageChoices
    ,activityType.MinChoices
    ,activityType.MaxChoices
    ,activityType.MinCorrectChoices
    ,activityType.MaxCorrectChoices
    ,activityType.ShuffleChoices
    ,category.Name as CategoryName
    ,status.Name as StatusName
from [Education].[ActivityType-Active] activityType
    inner join [Education].[ActivityCategory-Active] category on activityType.CategoryId = category.Id
    inner join [Education].[ActivityTypeStatus-Active] status on activityType.StatusId = status.Id
order by category.Name, activityType.Name