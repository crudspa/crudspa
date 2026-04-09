/*
merge into [Education].[ContentArea] as Target
using ( values
-- todo: Add values
) as Source
    (Id, Name, [Key], AppNavText, SuppressAudioChoices, MetalinguisticCategoryId)
on Target.Id = Source.Id

when matched then
update set
     Target.IsDeleted = 0
    ,Target.Name = Source.Name
    ,Target.[Key] = Source.[Key]
    ,Target.AppNavText = Source.AppNavText
    ,Target.SuppressAudioChoices = Source.SuppressAudioChoices
    ,Target.MetalinguisticCategoryId = Source.MetalinguisticCategoryId

when not matched by target then
insert (Id, Name, [Key], AppNavText, SuppressAudioChoices, MetalinguisticCategoryId)
values (Id, Name, [Key], AppNavText, SuppressAudioChoices, MetalinguisticCategoryId)

when not matched by source and Target.IsDeleted = 0 then
update set
     Target.IsDeleted = 1
;
*/