create view [Education].[ContentArea-Active] as

select contentArea.Id as Id
    ,contentArea.Name as Name
    ,contentArea.[Key] as [Key]
    ,contentArea.AppNavText as AppNavText
    ,contentArea.SuppressAudioChoices as SuppressAudioChoices
    ,contentArea.MetalinguisticCategoryId as MetalinguisticCategoryId
from [Education].[ContentArea] contentArea
where 1=1
    and contentArea.IsDeleted = 0