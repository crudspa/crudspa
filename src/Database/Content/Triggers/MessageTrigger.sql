create trigger [Content].[MessageTrigger] on [Content].[Message]
    for update
as

insert [Content].[Message] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,StageId
    ,Name
    ,PopulationId
    ,MessageTypeId
    ,EmailTemplateId
    ,SmsTemplateId
    ,Ordinal
    ,ActivationId
    ,DefinitionId
    ,MembershipId
    ,EmailId
    ,SmsId
    ,ActivationStageId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.StageId
    ,deleted.Name
    ,deleted.PopulationId
    ,deleted.MessageTypeId
    ,deleted.EmailTemplateId
    ,deleted.SmsTemplateId
    ,deleted.Ordinal
    ,deleted.ActivationId
    ,deleted.DefinitionId
    ,deleted.MembershipId
    ,deleted.EmailId
    ,deleted.SmsId
    ,deleted.ActivationStageId
from deleted