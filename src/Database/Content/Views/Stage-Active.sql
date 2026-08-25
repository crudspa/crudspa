create view [Content].[Stage-Active] as

select stage.Id as Id
    ,stage.CampaignId as CampaignId
    ,stage.PopulationId as PopulationId
    ,stage.Name as Name
    ,stage.Offset as Offset
    ,stage.Anchor as Anchor
    ,stage.MessageTypeId as MessageTypeId
    ,stage.EmailTemplateId as EmailTemplateId
    ,stage.SmsTemplateId as SmsTemplateId
    ,stage.WeekendAdjustment as WeekendAdjustment
    ,stage.SendTime as SendTime
    ,stage.Ordinal as Ordinal
from [Content].[Stage] stage
where 1=1
    and stage.IsDeleted = 0
    and stage.VersionOf = stage.Id