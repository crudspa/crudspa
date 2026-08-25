create trigger [Content].[StageTrigger] on [Content].[Stage]
    for update
as

insert [Content].[Stage] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,CampaignId
    ,PopulationId
    ,Name
    ,Offset
    ,Anchor
    ,MessageTypeId
    ,EmailTemplateId
    ,SmsTemplateId
    ,WeekendAdjustment
    ,SendTime
    ,Ordinal
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.CampaignId
    ,deleted.PopulationId
    ,deleted.Name
    ,deleted.Offset
    ,deleted.Anchor
    ,deleted.MessageTypeId
    ,deleted.EmailTemplateId
    ,deleted.SmsTemplateId
    ,deleted.WeekendAdjustment
    ,deleted.SendTime
    ,deleted.Ordinal
from deleted