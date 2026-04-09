/*
merge into [Education].[ActivityType] as Target
using ( values
-- todo: Add values
) as Source
    (Id, Name, [Key], DisplayView, CategoryId, StatusId, ContextGuidance, ChoiceGuidance, ExtraGuidance, SupportsContextText, RequiresContextText, SupportsContextAudio, RequiresContextAudio, SupportsContextImage, RequiresContextImage, SupportsExtraText, RequiresExtraText, SupportsChoices, RequiresCorrectChoices, SupportsAudioChoices, RequiresAudioChoices, RequiresDataChoices, RequiresImageChoices, SupportsTextChoices, RequiresTextChoices, RequiresLongerTextChoices, RequiresColumnOrdinal, RequiresTextOrImageChoices, MinChoices, MaxChoices, MinCorrectChoices, MaxCorrectChoices, ShuffleChoices)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.[Key] = Source.[Key]
    ,Target.DisplayView = Source.DisplayView
    ,Target.CategoryId = Source.CategoryId
    ,Target.StatusId = Source.StatusId
    ,Target.ContextGuidance = Source.ContextGuidance
    ,Target.ChoiceGuidance = Source.ChoiceGuidance
    ,Target.ExtraGuidance = Source.ExtraGuidance
    ,Target.SupportsContextText = Source.SupportsContextText
    ,Target.RequiresContextText = Source.RequiresContextText
    ,Target.SupportsContextAudio = Source.SupportsContextAudio
    ,Target.RequiresContextAudio = Source.RequiresContextAudio
    ,Target.SupportsContextImage = Source.SupportsContextImage
    ,Target.RequiresContextImage = Source.RequiresContextImage
    ,Target.SupportsExtraText = Source.SupportsExtraText
    ,Target.RequiresExtraText = Source.RequiresExtraText
    ,Target.SupportsChoices = Source.SupportsChoices
    ,Target.RequiresCorrectChoices = Source.RequiresCorrectChoices
    ,Target.SupportsAudioChoices = Source.SupportsAudioChoices
    ,Target.RequiresAudioChoices = Source.RequiresAudioChoices
    ,Target.RequiresDataChoices = Source.RequiresDataChoices
    ,Target.RequiresImageChoices = Source.RequiresImageChoices
    ,Target.SupportsTextChoices = Source.SupportsTextChoices
    ,Target.RequiresTextChoices = Source.RequiresTextChoices
    ,Target.RequiresLongerTextChoices = Source.RequiresLongerTextChoices
    ,Target.RequiresColumnOrdinal = Source.RequiresColumnOrdinal
    ,Target.RequiresTextOrImageChoices = Source.RequiresTextOrImageChoices
    ,Target.MinChoices = Source.MinChoices
    ,Target.MaxChoices = Source.MaxChoices
    ,Target.MinCorrectChoices = Source.MinCorrectChoices
    ,Target.MaxCorrectChoices = Source.MaxCorrectChoices
    ,Target.ShuffleChoices = Source.ShuffleChoices

when not matched by target then
insert (Id, Name, [Key], DisplayView, CategoryId, StatusId, ContextGuidance, ChoiceGuidance, ExtraGuidance, SupportsContextText, RequiresContextText, SupportsContextAudio, RequiresContextAudio, SupportsContextImage, RequiresContextImage, SupportsExtraText, RequiresExtraText, SupportsChoices, RequiresCorrectChoices, SupportsAudioChoices, RequiresAudioChoices, RequiresDataChoices, RequiresImageChoices, SupportsTextChoices, RequiresTextChoices, RequiresLongerTextChoices, RequiresColumnOrdinal, RequiresTextOrImageChoices, MinChoices, MaxChoices, MinCorrectChoices, MaxCorrectChoices, ShuffleChoices)
values (Id, Name, [Key], DisplayView, CategoryId, StatusId, ContextGuidance, ChoiceGuidance, ExtraGuidance, SupportsContextText, RequiresContextText, SupportsContextAudio, RequiresContextAudio, SupportsContextImage, RequiresContextImage, SupportsExtraText, RequiresExtraText, SupportsChoices, RequiresCorrectChoices, SupportsAudioChoices, RequiresAudioChoices, RequiresDataChoices, RequiresImageChoices, SupportsTextChoices, RequiresTextChoices, RequiresLongerTextChoices, RequiresColumnOrdinal, RequiresTextOrImageChoices, MinChoices, MaxChoices, MinCorrectChoices, MaxCorrectChoices, ShuffleChoices)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/